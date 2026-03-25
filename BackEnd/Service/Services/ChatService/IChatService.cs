using System;
using System.Threading.Tasks;
using Domain.Models;
using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Service.Services.ChatService
{
    public interface IChatService
    {
        Task<Guid> StartChatAsync(Guid currentUserId, Guid organizationId, Guid? productId);
        Task<ChatMessage> SendMessageAsync(Guid currentUserId, Guid chatRoomId, string content, MessageType type, decimal? proposedPrice, int? proposedQuantity, IFormFile? imageFile = null);
        Task<Guid> AcceptOfferAsync(Guid currentUserId, Guid offerMessageId);
        Task<string> RejectOfferAsync(Guid currentUserId, Guid offerMessageId);
        Task<string> DeleteMessageAsync(Guid currentUserId, Guid messageId);
        Task<string> MarkRoomAsReadAsync(Guid currentUserId, Guid chatRoomId);
        Task<string> CloseChatAsync(Guid currentUserId, Guid chatRoomId);
    }
}
