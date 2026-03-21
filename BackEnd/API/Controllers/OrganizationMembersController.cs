using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Controllers.Shared;
using Core.Features.OrganizationMember.Commands.Models;
using Core.Features.OrganizationMember.Queries.Model;
using Domain.MetaData;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{

    public class OrganizationMembersController : BaseApiController
    {
        [HttpGet(Router.OrganizationMember.GetOrganizationMembersByOrganizationId)]
        public async Task<IActionResult> GetOrganizationMembersByOrganizationId([FromRoute] Guid organizationId)
        {
            var result = await Mediator.Send(new GetOrganizationMembersByOrganizationModel(organizationId));
            return NewResult(result);
        }
        [HttpPut(Router.OrganizationMember.RemoveOrganizationMember)]
        public async Task<IActionResult> RemoveOrganizationMember([FromRoute] Guid organizationMemberId)
        {
            var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId)) return Unauthorized();

            var model = new RemoveOrganizationMemberModel(organizationMemberId, userId);
            var result = await Mediator.Send(model);
            return NewResult(result);
        }
    }
}