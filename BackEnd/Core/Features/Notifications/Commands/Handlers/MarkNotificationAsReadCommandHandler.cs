using System.Threading;
using System.Threading.Tasks;
using Core.Base;
using Core.Features.Notifications.Commands.Models;
using MediatR;
using Service.Services.NotificationService;

namespace Core.Features.Notifications.Commands.Handlers
{
    public class MarkNotificationAsReadCommandHandler : IRequestHandler<MarkNotificationAsReadCommand, Response<string>>
    {
        private readonly INotificationService _notificationService;

        public MarkNotificationAsReadCommandHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task<Response<string>> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
        {
            await _notificationService.MarkAsReadAsync(request.NotificationId, request.CurrentUserId);
            return new Response<string>("Notification marked as read.") { Succeeded = true };
        }
    }
}
