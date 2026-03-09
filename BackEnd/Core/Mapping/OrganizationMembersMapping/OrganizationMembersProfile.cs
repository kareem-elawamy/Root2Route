using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Mapping.OrganizationMembersMapping
{
    public partial class OrganizationMembersProfile : Profile
    {
        public OrganizationMembersProfile()
        {
            GetOrganizationMembersByOrganizationIdMapping();
        }
    }
}