using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.Models;
using Domain.Enums;
using Infrastructure.Repositories.ChatMessageRepository;
using Infrastructure.Repositories.ChatRoomRepository;
using Infrastructure.Repositories.OrderRepository;
using Infrastructure.Repositories.ProductRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Service.Hubs;
using Service.Services.FileService;
using Service.Services.NotificationService;

namespace Service.Services.ChatService
{
    public class ChatService : IChatService
    {
        private readonly IChatRoomRepository _chatRoomRepository;
        private readonly IChatMessageRepository _chatMessageRepository;
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly INotificationService _notificationService;
        private readonly IFileService _fileService;

        public ChatService(
            IChatRoomRepository chatRoomRepository,
            IChatMessageRepository chatMessageRepository,
            IProductRepository productRepository,
            IOrderRepository orderRepository,
            IHubContext<ChatHub> hubContext,
            INotificationService notificationService,
            IFileService fileService)
        {
            _chatRoomRepository = chatRoomRepository;
            _chatMessageRepository = chatMessageRepository;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _hubContext = hubContext;
            _notificationService = notificationService;
            _fileService = fileService;
        }

        public async Task<Guid> StartChatAsync(Guid currentUserId, Guid organizationId, Guid? productId)
        {
            if (productId.HasValue)
            {
                var productExists = await _productRepository.GetTableNoTracking()
                    .AnyAsync(p => p.Id == productId.Value && p.OrganizationId == organizationId);
                
                if (!productExists)
                    throw new InvalidOperationException("The specified product does not intrinsically belong to the target organization.");
            }

            var existingRoom = await _chatRoomRepository.GetTableNoTracking()
                .FirstOrDefaultAsync(c => c.BuyerId == currentUserId 
                                       && c.OrganizationId == organizationId 
                                       && c.ProductId == productId);

            if (existingRoom != null)
                return existingRoom.Id;

            var newRoom = new ChatRoom
            {
                Id = Guid.NewGuid(),
                BuyerId = currentUserId,
                OrganizationId = organizationId,
                ProductId = productId
            };

            await _chatRoomRepository.AddAsync(newRoom);
            await _chatRoomRepository.SaveChangesAsync();

            return newRoom.Id;
        }

        public async Task<ChatMessage> SendMessageAsync(Guid currentUserId, Guid chatRoomId, string content, MessageType type, decimal? proposedPrice, int? proposedQuantity, IFormFile? imageFile = null)
        {
            var room = await _chatRoomRepository.GetTableAsTracking()
                .Include(r => r.Organization)
                .ThenInclude(o => o.Members)
                .FirstOrDefaultAsync(r => r.Id == chatRoomId);

            if (room == null)
                throw new KeyNotFoundException("Chat room not found.");

            // Security: Closed room guard
            if (room.IsClosed)
                throw new InvalidOperationException("This chat room is closed. No further messages can be sent.");

            bool isBuyer = room.BuyerId == currentUserId;
            bool isSellerMember = room.Organization != null && room.Organization.Members.Any(m => m.UserId == currentUserId && m.IsActive);
            
            if (!isBuyer && !isSellerMember)
                throw new UnauthorizedAccessException("You are not a participant of this chat room.");

            // Handle image upload
            if (imageFile != null)
            {
                var imageUrl = await _fileService.UploadImageAsync("ChatImages", imageFile);
                content = imageUrl;
                type = MessageType.Image;
            }

            if (type == MessageType.NegotiationOffer)
            {
                if (room.ProductId == null)
                    throw new InvalidOperationException("Cannot negotiate in a general inquiry chat.");

                var product = await _productRepository.GetTableNoTracking().FirstOrDefaultAsync(p => p.Id == room.ProductId.Value);
                if (product == null)
                    throw new KeyNotFoundException("The associated product could not be explicitly located.");

                if (!proposedQuantity.HasValue || product.StockQuantity < proposedQuantity.Value)
                    throw new InvalidOperationException($"Insufficient product stock. Only {product.StockQuantity} available for negotiation.");
            }

            var message = new ChatMessage
            {
                Id = Guid.NewGuid(),
                ChatRoomId = chatRoomId,
                SenderId = currentUserId,
                Content = content,
                Type = type,
                ProposedPrice = proposedPrice,
                ProposedQuantity = proposedQuantity,
                SentAt = DateTime.UtcNow
            };

            room.LastMessageAt = message.SentAt;

            await _chatMessageRepository.AddAsync(message);
            await _chatRoomRepository.UpdateAsync(room);
            await _chatMessageRepository.SaveChangesAsync();

            await _hubContext.Clients.Group(chatRoomId.ToString())
                .SendAsync("ReceiveMessage", new 
                {
                    Id = message.Id,
                    ChatRoomId = message.ChatRoomId,
                    SenderId = message.SenderId,
                    Content = message.Content,
                    Type = message.Type,
                    IsRead = message.IsRead,
                    SentAt = message.SentAt,
                    ProposedPrice = message.ProposedPrice,
                    ProposedQuantity = message.ProposedQuantity,
                    RelatedOrderId = message.RelatedOrderId
                });

            try
            {
                if (room.BuyerId == currentUserId && room.Organization != null)
                {
                    foreach (var member in room.Organization.Members.Where(m => m.IsActive))
                        await _notificationService.SendPushNotificationAsync(member.UserId, "New B2B Message", content);
                }
                else
                {
                    await _notificationService.SendPushNotificationAsync(room.BuyerId, "New B2B Message", content);
                }
            }
            catch { }

            return message;
        }

        public async Task<Guid> AcceptOfferAsync(Guid currentUserId, Guid offerMessageId)
        {
            var offerMsg = await _chatMessageRepository.GetTableAsTracking()
                .Include(m => m.ChatRoom)
                .ThenInclude(r => r.Organization)
                .ThenInclude(o => o.Members)
                .FirstOrDefaultAsync(m => m.Id == offerMessageId);

            if (offerMsg == null || offerMsg.ChatRoom == null)
                throw new KeyNotFoundException("Negotiation offer not found.");

            if (offerMsg.Type != MessageType.NegotiationOffer)
                throw new InvalidOperationException("The specified message is not a valid negotiation offer.");

            var room = offerMsg.ChatRoom;

            // Security: Closed room guard
            if (room.IsClosed)
                throw new InvalidOperationException("This chat room is closed. Offers cannot be accepted.");

            bool isSellerMember = room.Organization != null && 
                                  room.Organization.Members.Any(m => m.UserId == currentUserId && m.IsActive);
            
            if (!isSellerMember)
                throw new UnauthorizedAccessException("Only active seller organization members can accept offers.");

            if (offerMsg.RelatedOrderId.HasValue)
                throw new InvalidOperationException("This offer has already been accepted and processed.");

            if (!offerMsg.ProposedPrice.HasValue || !offerMsg.ProposedQuantity.HasValue)
                throw new InvalidOperationException("Negotiation payload is missing absolute price/quantity metrics.");

            if (room.ProductId == null)
                throw new InvalidOperationException("This chat room is not explicitly bound to a product; cannot generate an automated order.");

            var hasNewerOffer = await _chatMessageRepository.GetTableNoTracking()
                .AnyAsync(m => m.ChatRoomId == room.Id 
                            && m.Type == MessageType.NegotiationOffer 
                            && m.SentAt > offerMsg.SentAt);

            if (hasNewerOffer)
                throw new InvalidOperationException("A newer negotiation offer is pending in this room. Outdated offers cannot be legally accepted.");

            await using var transaction = await _chatMessageRepository.BeginTransactionAsync();

            var product = await _productRepository.GetTableAsTracking().FirstOrDefaultAsync(p => p.Id == room.ProductId.Value);
            if (product == null)
                throw new KeyNotFoundException("The product no longer exists.");

            if (product.StockQuantity < offerMsg.ProposedQuantity.Value)
                throw new InvalidOperationException($"Insufficient operational stock. Only {product.StockQuantity} inherently available.");

            product.StockQuantity -= offerMsg.ProposedQuantity.Value;
            await _productRepository.UpdateAsync(product);

            var newOrder = new Order
            {
                Id = Guid.NewGuid(),
                BuyerId = room.BuyerId,
                OrganizationId = room.OrganizationId,
                TotalAmount = offerMsg.ProposedPrice.Value * offerMsg.ProposedQuantity.Value,
                Status = OrderStatus.Pending,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        productid = room.ProductId.Value,
                        Quantity = offerMsg.ProposedQuantity.Value,
                        UnitPrice = offerMsg.ProposedPrice.Value
                    }
                }
            };

            await _orderRepository.AddAsync(newOrder);

            offerMsg.RelatedOrderId = newOrder.Id;
            await _chatMessageRepository.UpdateAsync(offerMsg);

            var systemMessage = new ChatMessage
            {
                Id = Guid.NewGuid(),
                ChatRoomId = room.Id,
                SenderId = currentUserId,
                Content = $"Offer Accepted. Order Generated: {newOrder.Id}",
                Type = MessageType.OfferAccepted,
                RelatedOrderId = newOrder.Id,
                SentAt = DateTime.UtcNow
            };

            room.LastMessageAt = systemMessage.SentAt;
            await _chatMessageRepository.AddAsync(systemMessage);
            await _chatRoomRepository.UpdateAsync(room);

            await _chatMessageRepository.SaveChangesAsync();
            transaction.Commit();

            var broadcastPayload = new { 
                OrderId = newOrder.Id, 
                Message = new 
                {
                    Id = systemMessage.Id,
                    ChatRoomId = systemMessage.ChatRoomId,
                    SenderId = systemMessage.SenderId,
                    Content = systemMessage.Content,
                    Type = systemMessage.Type,
                    SentAt = systemMessage.SentAt,
                    RelatedOrderId = systemMessage.RelatedOrderId
                }
            };

            await _hubContext.Clients.Group(room.Id.ToString())
                .SendAsync("ReceiveOfferAccepted", broadcastPayload);

            try 
            {
                await _notificationService.SendPushNotificationAsync(room.BuyerId, "Offer Accepted", $"Your offer for {offerMsg.ProposedQuantity} units was securely accepted! Proceed to Secure Checkout.");
            }
            catch { }

            return newOrder.Id;
        }

        public async Task<string> MarkRoomAsReadAsync(Guid currentUserId, Guid chatRoomId)
        {
            var unreadMessages = await _chatMessageRepository.GetTableAsTracking()
                .Where(m => m.ChatRoomId == chatRoomId 
                         && m.SenderId != currentUserId 
                         && !m.IsRead)
                .ToListAsync();

            if (!unreadMessages.Any())
                return "No unread messages found.";

            foreach (var msg in unreadMessages)
            {
                msg.IsRead = true;
            }

            await _chatMessageRepository.UpdateRangeAsync(unreadMessages);
            await _chatMessageRepository.SaveChangesAsync();

            return $"Successfully marked {unreadMessages.Count} messages as read.";
        }

        public async Task<string> CloseChatAsync(Guid currentUserId, Guid chatRoomId)
        {
            var room = await _chatRoomRepository.GetTableAsTracking()
                .Include(r => r.Organization)
                .ThenInclude(o => o.Members)
                .FirstOrDefaultAsync(r => r.Id == chatRoomId);

            if (room == null)
                throw new KeyNotFoundException("Chat room not found.");

            bool isBuyer = room.BuyerId == currentUserId;
            bool isSellerMember = room.Organization != null && room.Organization.Members.Any(m => m.UserId == currentUserId && m.IsActive);

            if (!isBuyer && !isSellerMember)
                throw new UnauthorizedAccessException("You are not a participant of this chat room.");

            if (room.IsClosed)
                throw new InvalidOperationException("This chat room is already closed.");

            room.IsClosed = true;
            await _chatRoomRepository.UpdateAsync(room);
            await _chatRoomRepository.SaveChangesAsync();

            await _hubContext.Clients.Group(chatRoomId.ToString())
                .SendAsync("ChatRoomClosed", new { ChatRoomId = chatRoomId, ClosedBy = currentUserId });

            return "Chat room has been successfully closed.";
        }

        public async Task<string> RejectOfferAsync(Guid currentUserId, Guid offerMessageId)
        {
            var offerMsg = await _chatMessageRepository.GetTableAsTracking()
                .Include(m => m.ChatRoom)
                .ThenInclude(r => r.Organization)
                .ThenInclude(o => o.Members)
                .FirstOrDefaultAsync(m => m.Id == offerMessageId);

            if (offerMsg == null || offerMsg.ChatRoom == null)
                throw new KeyNotFoundException("Negotiation offer not found.");

            if (offerMsg.Type != MessageType.NegotiationOffer)
                throw new InvalidOperationException("The specified message is not a valid negotiation offer.");

            var room = offerMsg.ChatRoom;

            if (room.IsClosed)
                throw new InvalidOperationException("This chat room is closed. Offers cannot be rejected.");

            bool isSellerMember = room.Organization != null &&
                                  room.Organization.Members.Any(m => m.UserId == currentUserId && m.IsActive);

            if (!isSellerMember)
                throw new UnauthorizedAccessException("Only active seller organization members can reject offers.");

            if (offerMsg.RelatedOrderId.HasValue)
                throw new InvalidOperationException("This offer has already been accepted and cannot be rejected.");

            // Check if a rejection system message already exists for this offer
            var alreadyRejected = await _chatMessageRepository.GetTableNoTracking()
                .AnyAsync(m => m.ChatRoomId == room.Id
                            && m.Type == MessageType.OfferRejected
                            && m.SentAt > offerMsg.SentAt
                            && m.SenderId == currentUserId);

            if (alreadyRejected)
                throw new InvalidOperationException("This offer has already been rejected.");

            var systemMessage = new ChatMessage
            {
                Id = Guid.NewGuid(),
                ChatRoomId = room.Id,
                SenderId = currentUserId,
                Content = "Offer was rejected.",
                Type = MessageType.OfferRejected,
                SentAt = DateTime.UtcNow
            };

            room.LastMessageAt = systemMessage.SentAt;

            await _chatMessageRepository.AddAsync(systemMessage);
            await _chatRoomRepository.UpdateAsync(room);
            await _chatMessageRepository.SaveChangesAsync();

            await _hubContext.Clients.Group(room.Id.ToString())
                .SendAsync("ReceiveOfferRejected", new
                {
                    OfferMessageId = offerMessageId,
                    Message = new
                    {
                        Id = systemMessage.Id,
                        ChatRoomId = systemMessage.ChatRoomId,
                        SenderId = systemMessage.SenderId,
                        Content = systemMessage.Content,
                        Type = systemMessage.Type,
                        SentAt = systemMessage.SentAt
                    }
                });

            try
            {
                await _notificationService.SendPushNotificationAsync(room.BuyerId, "Offer Rejected", "Your negotiation offer was rejected by the seller.");
            }
            catch { }

            return "Offer has been rejected successfully.";
        }

        public async Task<string> DeleteMessageAsync(Guid currentUserId, Guid messageId)
        {
            var message = await _chatMessageRepository.GetTableAsTracking()
                .FirstOrDefaultAsync(m => m.Id == messageId);

            if (message == null)
                throw new KeyNotFoundException("Message not found.");

            if (message.SenderId != currentUserId)
                throw new UnauthorizedAccessException("You can only delete your own messages.");

            // Soft delete: preserve audit trail
            message.Content = "This message was deleted.";
            message.Type = MessageType.Deleted;

            await _chatMessageRepository.UpdateAsync(message);
            await _chatMessageRepository.SaveChangesAsync();

            await _hubContext.Clients.Group(message.ChatRoomId.ToString())
                .SendAsync("ReceiveMessageDeleted", new
                {
                    MessageId = messageId,
                    ChatRoomId = message.ChatRoomId
                });

            return "Message deleted successfully.";
        }
    }
}
