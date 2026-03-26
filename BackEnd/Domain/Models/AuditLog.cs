using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    public class AuditLog : BaseEntity
    {
        public Guid UserId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string EntityName { get; set; } = string.Empty;
        public string OldValues { get; set; } = string.Empty;
        public string NewValues { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(UserId))]
        public ApplicationUser? User { get; set; }
    }
}
