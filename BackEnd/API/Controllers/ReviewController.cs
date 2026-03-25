using System;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Controllers.Shared;
using Core.Features.Reviews.Commands.Models;
using Core.Features.Reviews.Queries.Models;
using Domain.MetaData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Authorize]
    public class ReviewController : BaseApiController
    {
        [HttpPost(Router.Review.Add)]
        public async Task<IActionResult> AddReview([FromBody] AddReviewCommand command)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            command.CurrentUserId = userId.Value;
            var response = await Mediator.Send(command);
            return NewResult(response);
        }

        [HttpGet(Router.Review.GetByOrganization)]
        public async Task<IActionResult> GetOrganizationReviews([FromRoute] Guid orgId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var query = new GetOrganizationReviewsQuery
            {
                OrganizationId = orgId,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var response = await Mediator.Send(query);
            return NewResult(response);
        }

        #region Helpers
        private Guid? GetCurrentUserId()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString)) return null;
            return Guid.Parse(userIdString);
        }
        #endregion
    }
}
