using System.Security.Claims;
using API.Controllers.Shared;
using Core.Features.Organization.Commands.Models;
using Core.Features.Organization.Queries.Models;
using Domain.Enums;
using Domain.MetaData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Authorize] // مهم جدًا
    public class OrganizationController : BaseApiController
    {
        #region Queries

        [HttpGet(Router.Organization.GetAllOrganizations)]
        public async Task<IActionResult> GetAllOrganization()
        {
            var response = await Mediator.Send(new GetAllOrganizations());
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
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var response = await Mediator.Send(
                new GetAllOrganizationsByUserId(userId.Value));

            return NewResult(response);
        }

        [HttpGet(Router.Organization.GetStatistics)]
        public async Task<IActionResult> GetStatistics([FromRoute] Guid id)
        {
            var response = await Mediator.Send(
                new GetOrganizationStatisticsQuery(id));

            return NewResult(response);
        }

        #endregion

        #region Commands

        [HttpPost(Router.Organization.CreateOrganization)]
        public async Task<IActionResult> CreateOrganization(
            [FromForm] CreateOrganizationCommand command)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            command.OwnerId = userId.Value;

            var response = await Mediator.Send(command);
            return NewResult(response);
        }

        [HttpPut(Router.Organization.UpdateById)]
        public async Task<IActionResult> UpdateOrganization(
            [FromForm] UpdateOrganizations command)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            command.OwnerId = userId.Value;

            var response = await Mediator.Send(command);
            return NewResult(response);
        }

        [HttpDelete(Router.Organization.SoftDelete)]
        public async Task<IActionResult> SoftDelete([FromRoute] Guid id)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var command = new SoftDeleteOrganizationCommand(id, userId.Value);

            var response = await Mediator.Send(command);
            return NewResult(response);
        }

        [HttpPut(Router.Organization.Restore)]
        public async Task<IActionResult> Restore([FromRoute] Guid id)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var command = new RestoreOrganizationCommand(id, userId.Value);

            var response = await Mediator.Send(command);
            return NewResult(response);
        }

        [HttpPut(Router.Organization.ChangeOwner)]
        public async Task<IActionResult> ChangeOwner(
            [FromRoute] Guid id,
            [FromBody] Guid newOwnerId)
        {
            var currentOwnerId = GetCurrentUserId();
            if (currentOwnerId == null) return Unauthorized();

            var command = new ChangeOwnerCommand(
                id,
                currentOwnerId.Value,
                newOwnerId);

            var response = await Mediator.Send(command);
            return NewResult(response);
        }

        [HttpPut(Router.Organization.UpdateStatus)]
        public async Task<IActionResult> UpdateStatus(
            [FromRoute] Guid id,
            [FromBody] OrganizationStatus newStatus)
        {
            var command = new UpdateOrganizationStatusCommand(id, newStatus);

            var response = await Mediator.Send(command);
            return NewResult(response);
        }


        #endregion


        #region Helpers

        private Guid? GetCurrentUserId()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString))
                return null;

            return Guid.Parse(userIdString);
        }

        #endregion
    }
}