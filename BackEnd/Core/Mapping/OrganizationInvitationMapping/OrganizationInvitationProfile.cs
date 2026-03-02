using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Mapping.OrganizationInvitationMapping
{
    public partial class OrganizationInvitationProfile : Profile
    {
        public OrganizationInvitationProfile()
        {
            GetAllInvitationsForUserMapping();
            SendInvitationCommandMapping();
            GetAllInvitationsForOrganizationMapping();
        }
    }
}