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
            var model = new RemoveOrganizationMemberModel(organizationMemberId);
            var result = await Mediator.Send(model);
            return NewResult(result);
        }
    }
}