using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Features.Organization.Queries.Result;
using Core.Features.OrganizationInvitation.Queries.Models;
using Core.Features.OrganizationInvitation.Queries.Ressult;
using Microsoft.AspNetCore.Mvc;

namespace Core.Features.OrganizationInvitation.Queries.Handlers
{
    public class GetInvitationsByOrganizationHandlerQueray : ResponseHandler, IRequestHandler<GetInvitationsByOrganizationModelQueray, Response<List<OrganizationInvitationResult>>>
    {
        private readonly IOrganizationInvitationService _invitationService;
        private readonly IMapper _mapper;
        public GetInvitationsByOrganizationHandlerQueray(IMapper mapper,IOrganizationInvitationService invitationService)
        {
            _mapper=mapper;
            _invitationService = invitationService;
        }

        public async Task<Response<List<OrganizationInvitationResult>>> Handle(GetInvitationsByOrganizationModelQueray request, CancellationToken cancellationToken)
        {
            var result = await _invitationService.GetInvitationsByOrganizationIdAsync(request.OrganizationId);
            if (result == null || !result.Any())
                return NotFound<List<OrganizationInvitationResult>>("No invitations found");

            var mappedResult = _mapper
        .Map<List<OrganizationInvitationResult>>(result);
            return Success(mappedResult);
            throw new NotImplementedException();
        }
    }
}