using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Features.OrganizationInvitation.Queries.Ressult;

namespace Core.Features.OrganizationInvitation.Commands.Models
{
    public class GetAllInvitationsForUserModel : IRequest<Response<List<OrganizationInvitationResult>>>
    {
        public Guid UserId { get; set; }
        
        public GetAllInvitationsForUserModel(Guid userId)
        {
            UserId = userId;
        }
    }
}