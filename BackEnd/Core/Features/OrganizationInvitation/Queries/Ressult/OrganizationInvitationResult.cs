using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enums;

namespace Core.Features.OrganizationInvitation.Queries.Ressult
{
    public class OrganizationInvitationResult
    {
        public Guid Id { get; set; }
        public Guid OrganizationId { get; set; }
        public string Email { get; set; }
        public DateTime SentAt { get; set; }
        public InvitationStatus Status { get; set; }
        public DateTime ExpiryDate { get; set; } = DateTime.UtcNow.AddDays(3);
        public string OrganizationName { get; set; }
        public string RoleName { get; set; }

    }
}