using Domain.Common;
using System;

namespace Domain.Models
{
    public class Notification : BaseEntity
    {
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; } = false;
        public Guid? RelatedEntityId { get; set; }
    }
}
