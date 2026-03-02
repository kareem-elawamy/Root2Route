using System;
using System.ComponentModel.DataAnnotations;
using Domain.Common;
using Domain.Enums;

namespace Domain.Models
{
    public class OrganizationInvitation : BaseEntity
    {
        [Required, EmailAddress]
        public string Email { get; set; } = null!; // الإيميل اللي بنبعتله

        // مين اللي بعت الدعوة؟ (عشان نعرف مين الـ HR اللي بعتها)
        public Guid SenderId { get; set; }

        // الدعوة لأي مؤسسة؟
        public Guid OrganizationId { get; set; }
        public Organization? Organization { get; set; }

        // الدعوة على وظيفة إيه؟
        public Guid RoleId { get; set; }
        public OrganizationRole? Role { get; set; }

        // كود الدعوة السري (بيتبعت في اللينك)
        public string Token { get; set; } = Guid.NewGuid().ToString();

        public DateTime ExpiryDate { get; set; } 

        public InvitationStatus Status { get; set; } = InvitationStatus.Pending;
    }
}
