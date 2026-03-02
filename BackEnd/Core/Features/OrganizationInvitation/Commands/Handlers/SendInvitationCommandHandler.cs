using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Features.OrganizationInvitation.Commands.Models;


namespace Core.Features.OrganizationInvitation.Commands.Handlers
{
    public class SendInvitationCommandHandler : ResponseHandler, IRequestHandler<SendInvitationCommandModel, Response<string>>
    {
        private readonly IMapper _mapper;
        private readonly IOrganizationInvitationService _invitationService;
        public SendInvitationCommandHandler(IMapper mapper, IOrganizationInvitationService invitationService)
        {
            _mapper = mapper;
            _invitationService = invitationService;
        }



        public async Task<Response<string>> Handle(SendInvitationCommandModel request, CancellationToken cancellationToken)
        {
            var invitation = _mapper.Map<Domain.Models.OrganizationInvitation>(request);
            var result = await _invitationService.SendInvitationAsync(invitation);

            return result switch
            {
                InvitationResult.Success => Success("Invitation sent successfully"),
                InvitationResult.NotFound => NotFound<string>("Organization not found"),
                InvitationResult.InvalidUser => BadRequest<string>("Invalid user"),
                InvitationResult.InvalidRole => BadRequest<string>("Invalid role"),
                InvitationResult.AlreadyInvited => BadRequest<string>("User already invited"),
                _ => BadRequest<string>("Failed to send invitation")
            };
        }
    }
}