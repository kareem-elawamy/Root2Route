using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Controllers.Shared;
using Core.Features.OrganizationRole.Commands.Models;
using Core.Features.OrganizationRole.Queries.Models;
using Core.Filters;
using Domain.Constants;
using Domain.MetaData;
using Microsoft.AspNetCore.Mvc;
using static Domain.Constants.Permissions;

namespace API.Controllers
{
    [ApiController]
    public class OrganizationRoleController : BaseApiController
    {
        #region permissions
        [HttpGet(Router.OrganizationRole.GetSystemPermissions)]
        public async Task<IActionResult> GetSystemPermissions()
        {
            var result = await Mediator.Send(new GetAllSystemPermissionsQuery());
            return Ok(result);
        }
        #endregion
        [HttpPost(Router.OrganizationRole.CreateOrganizationRole)]
        [HasPermission(Permissions.Roles.Create)]
        public async Task<IActionResult> CreateOrganizationRole([FromBody] AddOrganizationRoleCommand command, [FromHeader(Name = "X-Organization-Id")] Guid organizationId)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdString == null) return Unauthorized();

            command.RequesterUserId = Guid.Parse(userIdString);
            command.OrganizationId = organizationId;

            var result = await Mediator.Send(command);
            return NewResult(result);
        }
        [HttpGet(Router.OrganizationRole.GetOrganizationRolesByOrganizationId)]
        public async Task<IActionResult> GetOrganizationRolesByOrganizationId([FromRoute] Guid organizationId)
        {
            var query = new GetOrganizationRolesByOrganizationIdModel(organizationId);
            var response = await Mediator.Send(query);
            return NewResult(response);
        }
    }
}