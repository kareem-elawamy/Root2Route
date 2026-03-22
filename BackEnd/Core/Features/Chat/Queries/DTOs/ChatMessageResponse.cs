using System;
using Domain.Enums;

namespace Core.Features.Chat.Queries.DTOs
{
    public class ChatMessageResponse
    {
        public Guid Id { get; set; }
        public Guid ChatRoomId { get; set; }
        public Guid SenderId { get; set; }
        public string Content { get; set; } = string.Empty;
        public MessageType Type { get; set; }
        public bool IsRead { get; set; }
        public DateTime SentAt { get; set; }
        public decimal? ProposedPrice { get; set; }
        public int? ProposedQuantity { get; set; }
        public Guid? RelatedOrderId { get; set; }
    }
}
