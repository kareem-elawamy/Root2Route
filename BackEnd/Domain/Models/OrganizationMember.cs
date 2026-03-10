using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class OrganizationMember : BaseEntity
    {
        public Guid OrganizationId { get; set; }
        public Organization? Organization { get; set; }
        public Guid UserId { get; set; }
        public ApplicationUser? User { get; set; }
        public ICollection<OrganizationRole> OrganizationRoles { get; set; } = new List<OrganizationRole>();
        public bool IsActive { get; set; } = true;
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    }
}
