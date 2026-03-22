using System;
using System.Threading.Tasks;
using Domain.Models;
using Domain.Enums;

namespace Service.Services.ChatService
{
    public interface IChatService
    {
        Task<Guid> StartChatAsync(Guid currentUserId, Guid organizationId, Guid? productId);
        Task<ChatMessage> SendMessageAsync(Guid currentUserId, Guid chatRoomId, string content, MessageType type, decimal? proposedPrice, int? proposedQuantity);
        Task<Guid> AcceptOfferAsync(Guid currentUserId, Guid offerMessageId);
        Task<string> MarkRoomAsReadAsync(Guid currentUserId, Guid chatRoomId);
    }
}
