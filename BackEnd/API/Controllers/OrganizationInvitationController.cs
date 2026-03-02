using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Controllers.Shared;
using Core.Features.OrganizationInvitation.Commands.Models;
using Core.Features.OrganizationInvitation.Queries.Models;
using Domain.MetaData;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    public class OrganizationInvitationController : BaseApiController
    {
        [HttpGet(Router.OrganizationInvitation.GetAllInvitationsForUser)]
        public async Task<IActionResult> GetAllInvitationsForUser()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var result = await Mediator.Send(new GetAllInvitationsForUserModel(userId.Value));
            return NewResult(result);
        }

        [HttpPost(Router.OrganizationInvitation.SendInvitation)]
        public async Task<IActionResult> SendInvitation(SendInvitationCommandModel model)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();
            model = model with { SenderId = userId.Value };

            var result = await Mediator.Send(model);
            return NewResult(result);
        }
        [HttpPost(Router.OrganizationInvitation.AcceptInvitation)]
        public async Task<IActionResult> AcceptInvitation(Guid InvitationId)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();
            var model = new AcceptInvitationCommandModel(
                 userId.Value,
                 InvitationId
            );
            var result = await Mediator.Send(model);
            return NewResult(result);
        }
        [HttpPost(Router.OrganizationInvitation.Revoken)]
        public async Task<IActionResult> RevokenInvitation(Guid InvitationId)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();
            var model = new RevokeInvitationCommandModel(
                 userId.Value,
                 InvitationId
            );
            var result = await Mediator.Send(model);
            return NewResult(result);
        }
        [HttpGet(Router.OrganizationInvitation.Prefix)]
        public async Task<IActionResult> OrganizationInvitation(Guid organizationId)
        {

            var result = await Mediator.Send(new GetInvitationsByOrganizationModelQueray(organizationId));
            return NewResult(result);
        }
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