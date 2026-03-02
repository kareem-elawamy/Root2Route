using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Features.OrganizationInvitation.Commands.Models;
using Core.Features.OrganizationInvitation.Queries.Ressult;
using Domain.Enums;

namespace Core.Features.OrganizationInvitation.Queries.Handlers
{
    public class GetAllInvitationsForUserHandler : ResponseHandler, IRequestHandler<GetAllInvitationsForUserModel, Response<List<OrganizationInvitationResult>>>
    {
        private readonly IOrganizationInvitationService _invitationService;
        private readonly IMapper _mapper;
        public GetAllInvitationsForUserHandler(IMapper mapper, IOrganizationInvitationService invitationService)
        {
            _mapper = mapper;
            _invitationService = invitationService;
        }
        public async Task<Response<List<OrganizationInvitationResult>>> Handle(GetAllInvitationsForUserModel request, CancellationToken cancellationToken)
        {
            var result = await _invitationService.GetAllInvitationsForUserAsync(request.UserId);
            if (result == null || !result.Any())
                return NotFound<List<OrganizationInvitationResult>>("No invitations found");

            var mappedResult = _mapper
        .Map<List<OrganizationInvitationResult>>(result);
            return Success(mappedResult);

        }
    }
}