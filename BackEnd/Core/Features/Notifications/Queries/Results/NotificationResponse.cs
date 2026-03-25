using System;

namespace Core.Features.Notifications.Queries.Results
{
    public class NotificationResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? RelatedEntityId { get; set; }
    }
}
