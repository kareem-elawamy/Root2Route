using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Features.OrganizationInvitation.Commands.Handlers
{
    public class AcceptInvitationCommandHandler : ResponseHandler, IRequestHandler<AcceptInvitationCommandModel, Response<string>>
    {
        private readonly IOrganizationInvitationService _invitationService;
        public AcceptInvitationCommandHandler(IOrganizationInvitationService invitationService)
        {
            _invitationService = invitationService;

        }
        public async Task<Response<string>> Handle(AcceptInvitationCommandModel request, CancellationToken cancellationToken)
        {
            var result = await _invitationService.AcceptInvitationAsync(request.InvitationId, request.UserId);
            return result switch
            {
                InvitationResult.NotFound => NotFound<string>("Not Found Invitation"),
                InvitationResult.Expired => BadRequest<string>("Invitation Expired"),
                InvitationResult.AlreadyMember => BadRequest<string>("Already Member"),
                InvitationResult.Success => Success("Accept Invitation Successfully"),
                _ => BadRequest<string>("Failed to Accept invitation")
            };
        }
    }
}