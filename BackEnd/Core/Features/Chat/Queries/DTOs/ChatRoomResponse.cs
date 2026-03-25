using System;

namespace Core.Features.Chat.Queries.DTOs
{
    public class ChatRoomResponse
    {
        public Guid Id { get; set; }
        public Guid BuyerId { get; set; }
        public Guid OrganizationId { get; set; }
        public string OrganizationName { get; set; } = string.Empty;
        public string? ProductName { get; set; }
        public string LastMessageSnippet { get; set; } = string.Empty;
        public DateTime LastMessageAt { get; set; }
        public int UnreadCount { get; set; }
    }
}
