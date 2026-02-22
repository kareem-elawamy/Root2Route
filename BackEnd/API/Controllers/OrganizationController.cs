using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Controllers.Shared;
using Core.Features.Organization.Commands.Models;
using Core.Features.Organization.Queries.Models;
using Core.Features.Organization.Queries.Result;
using Domain.MetaData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    public class OrganizationController : BaseApiController
    {

        [HttpGet(Router.Organization.GetAllOrganizations)]
        public async Task<IActionResult> GetAllOrganization()
        {

            var response = await Mediator.Send(new GetAllOrganizations());
            return NewResult(response);
        }
        [HttpPost(Router.Organization.CreateOrganization)]
        public async Task<IActionResult> CreateOrganization([FromForm] CreateOrganizationCommand command)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdString == null) return Unauthorized();
            var ownerId = Guid.Parse(userIdString);
            command.OwnerId = ownerId;

            var response = await Mediator.Send(command);
            return NewResult(response);
        }
        [HttpGet(Router.Organization.GetById)]
        public async Task<IActionResult> GetOrganizationById([FromRoute] Guid id)
        {
            var response = await Mediator.Send(new GetOrganizationsById(id));
            return NewResult(response);
        }
        [HttpGet(Router.Organization.GetMyOrganizations)]
        public async Task<IActionResult> GetMyOrganizations()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdString == null) return Unauthorized();
            var ownerId = Guid.Parse(userIdString);
            var response = await Mediator.Send(new GetAllOrganizationsByUserId(ownerId));
            return NewResult(response);
        }
        [HttpPut(Router.Organization.UpdateById)]
        public async Task<IActionResult> UpdateOrganization([FromForm] UpdateOrganizations update,[FromRoute] Guid organizationId)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdString == null) return Unauthorized();
            var ownerId = Guid.Parse(userIdString);
            var response = await Mediator.Send(new UpdateOrganizations(ownerId,organizationId));
                        return NewResult(response);


        }

    }
}