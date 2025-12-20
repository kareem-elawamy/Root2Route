using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class OrganizationRole : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsSystemDefault { get; set; }
        public Guid OrganizationId { get; set; }
        public Organization? Organization { get; set; } 
        public ICollection<OrganizationRolePermission> Permissions { get; set; } = new List<OrganizationRolePermission>();

        
    }
}