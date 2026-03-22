using System.Threading;
using System.Threading.Tasks;
using Core.Base;
using Core.Features.Notifications.Commands.Models;
using MediatR;
using Service.Services.NotificationService;

namespace Core.Features.Notifications.Commands.Handlers
{
    public class MarkAllNotificationsAsReadCommandHandler : IRequestHandler<MarkAllNotificationsAsReadCommand, Response<string>>
    {
        private readonly INotificationService _notificationService;

        public MarkAllNotificationsAsReadCommandHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task<Response<string>> Handle(MarkAllNotificationsAsReadCommand request, CancellationToken cancellationToken)
        {
            await _notificationService.MarkAllAsReadAsync(request.CurrentUserId);
            return new Response<string>("All notifications marked as read.") { Succeeded = true };
        }
    }
}
