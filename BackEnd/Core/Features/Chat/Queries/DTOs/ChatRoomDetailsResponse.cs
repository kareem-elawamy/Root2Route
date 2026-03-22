using System;

namespace Core.Features.Chat.Queries.DTOs
{
    public class ChatRoomDetailsResponse
    {
        public string OrganizationName { get; set; } = string.Empty;
        public string? OrganizationLogo { get; set; }
        public string? ProductName { get; set; }
        public decimal? ProductOriginalPrice { get; set; }
        public bool IsProductChat { get; set; }
        public bool IsClosed { get; set; }
    }
}
