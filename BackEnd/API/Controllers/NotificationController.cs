using System;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Controllers.Shared;
using Core.Features.Notifications.Commands.Models;
using Core.Features.Notifications.Queries.Models;
using Domain.MetaData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Authorize]
    public class NotificationController : BaseApiController
    {
        [HttpGet(Router.Notification.GetMy)]
        public async Task<IActionResult> GetMyNotifications([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var query = new GetMyNotificationsQuery
            {
                CurrentUserId = userId.Value,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var response = await Mediator.Send(query);
            return NewResult(response);
        }

        [HttpGet(Router.Notification.UnreadCount)]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var query = new GetUnreadNotificationsCountQuery { CurrentUserId = userId.Value };
            var response = await Mediator.Send(query);
            return NewResult(response);
        }

        [HttpPut(Router.Notification.MarkRead)]
        public async Task<IActionResult> MarkAsRead([FromRoute] Guid id)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var command = new MarkNotificationAsReadCommand
            {
                NotificationId = id,
                CurrentUserId = userId.Value
            };

            var response = await Mediator.Send(command);
            return NewResult(response);
        }

        [HttpPut(Router.Notification.MarkAllRead)]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var command = new MarkAllNotificationsAsReadCommand { CurrentUserId = userId.Value };
            var response = await Mediator.Send(command);
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
