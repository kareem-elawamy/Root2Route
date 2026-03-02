using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Features.OrganizationInvitation.Queries.Ressult;

namespace Core.Features.OrganizationInvitation.Queries.Models
{
    public class GetInvitationsByOrganizationModelQueray : IRequest<Response<List<OrganizationInvitationResult>>>
    {
        public Guid OrganizationId { get; set; }

        public GetInvitationsByOrganizationModelQueray(Guid organizationId)
        {
            OrganizationId = organizationId;
        }
    }
}