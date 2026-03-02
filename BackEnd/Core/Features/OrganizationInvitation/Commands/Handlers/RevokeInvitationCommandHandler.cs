
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Org.BouncyCastle.Ocsp;

namespace Core.Features.OrganizationInvitation.Commands.Handlers
{
    public class RevokeInvitationCommandHandler : ResponseHandler, IRequestHandler<RevokeInvitationCommandModel, Response<string>>
    {
        private readonly IOrganizationInvitationService _invitationService;
        public RevokeInvitationCommandHandler(IOrganizationInvitationService invitationService)
        {
            _invitationService = invitationService;

        }
        public async Task<Response<string>> Handle(RevokeInvitationCommandModel request, CancellationToken cancellationToken)
        {
            var result = await _invitationService.RevokeInvitationAsync(request.InvitationId,request.UserId);
            return result switch
            {
                InvitationResult.NotFound => NotFound<string>("Not Found Invitaion"),
                InvitationResult.InvalidUser => Unauthorized<string>("Invalid User"),
                InvitationResult.Success => Success("Invitation Revoken Success"),
                _ => BadRequest<string>("Failed to Revoken invitation")
            };
        }
    }
}