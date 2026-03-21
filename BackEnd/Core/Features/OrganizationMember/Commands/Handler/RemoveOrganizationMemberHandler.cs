using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Features.OrganizationMember.Commands.Models;
using Service.Services.OrganizationMemberService;

namespace Core.Features.OrganizationMember.Commands.Handler
{
    public class RemoveOrganizationMemberHandler : ResponseHandler, IRequestHandler<RemoveOrganizationMemberModel, Response<string>>
    {
        private readonly IOrganizationMemberService _organizationMemberService;
        public RemoveOrganizationMemberHandler(IOrganizationMemberService organizationMemberService)
        {
            _organizationMemberService = organizationMemberService;
        }
        public async Task<Response<string>> Handle(RemoveOrganizationMemberModel request, CancellationToken cancellationToken)
        {
            var result = await _organizationMemberService.RemoveOrganizationMemberAsync(request.OrganizationMemberId, request.CurrentUserId);
            if (result.StartsWith("Failed"))
            {
                return BadRequest<string>("Failed to remove organization member: " + result);
            }
            return Success<string>("Organization member removed successfully.");
        }
    }
}