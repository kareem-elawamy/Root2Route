using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Controllers.Shared;
using Core.Features.Organization.Queries.Models;
using Core.Features.Organization.Queries.Result;
using Domain.MetaData;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    public class OrganizationController : BaseApiController
    {

        [HttpGet(Router.Organization.GetAllOwnerOrganizations)]
        public async Task<IActionResult> GetAllOrganization()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdString == null) return Unauthorized();
            var ownerId = Guid.Parse(userIdString);

            var response = await Mediator.Send(new GetAllOwnerOrganizations(ownerId)); return NewResult(response);
        }
    }
}