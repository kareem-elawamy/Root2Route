using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Features.OrganizationRole.Queries.Result
{
    public class GetOrganizationRolesByOrganizationIdResult
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsSystemDefault { get; set; }
        public Guid OrganizationId { get; set; }
        public List<OrganizationRolePermissionsList> Permissions { get; set; } =
            new List<OrganizationRolePermissionsList>();
       
        
    }
}