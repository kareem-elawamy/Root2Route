using System.Threading;
using System.Threading.Tasks;
using Core.Base;
using Core.Features.Notifications.Queries.Models;
using MediatR;
using Service.Services.NotificationService;

namespace Core.Features.Notifications.Queries.Handlers
{
    public class GetUnreadNotificationsCountQueryHandler : IRequestHandler<GetUnreadNotificationsCountQuery, Response<int>>
    {
        private readonly INotificationService _notificationService;

        public GetUnreadNotificationsCountQueryHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task<Response<int>> Handle(GetUnreadNotificationsCountQuery request, CancellationToken cancellationToken)
        {
            var count = await _notificationService.GetUnreadCountAsync(request.CurrentUserId);
            return new Response<int>(count) { Succeeded = true };
        }
    }
}
