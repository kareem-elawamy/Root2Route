using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Features.OrganizationMember.Queries.Result
{
    public class GetOrganizationMembersByOrganizationIdResult
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public bool IsActive { get; set; }
        public DateTime JoinedAt { get; set; }
    }
}