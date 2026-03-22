using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Base;
using Core.Features.Notifications.Queries.Models;
using Core.Features.Notifications.Queries.Results;
using MediatR;
using Service.Services.NotificationService;

namespace Core.Features.Notifications.Queries.Handlers
{
    public class GetMyNotificationsQueryHandler : IRequestHandler<GetMyNotificationsQuery, Response<List<NotificationResponse>>>
    {
        private readonly INotificationService _notificationService;

        public GetMyNotificationsQueryHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task<Response<List<NotificationResponse>>> Handle(GetMyNotificationsQuery request, CancellationToken cancellationToken)
        {
            request.PageSize = request.PageSize > 50 ? 50 : request.PageSize;

            var (notifications, totalCount) = await _notificationService.GetMyNotificationsAsync(
                request.CurrentUserId,
                request.PageNumber,
                request.PageSize);

            var mapped = notifications.Select(n => new NotificationResponse
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Message,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt,
                RelatedEntityId = n.RelatedEntityId
            }).ToList();

            return new Response<List<NotificationResponse>>(mapped) 
            { 
                Succeeded = true,
                Meta = new { TotalCount = totalCount, request.PageNumber, request.PageSize }
            };
        }
    }
}
