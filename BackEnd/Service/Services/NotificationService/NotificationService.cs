using System;
using System.Threading.Tasks;
using Domain.Models;
using Infrastructure.Repositories.NotificationRepository;
using Microsoft.AspNetCore.SignalR;
using Service.Hubs;

namespace Service.Services.NotificationService
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IHubContext<ChatHub> _hubContext;

        public NotificationService(
            INotificationRepository notificationRepository,
            IHubContext<ChatHub> hubContext)
        {
            _notificationRepository = notificationRepository;
            _hubContext = hubContext;
        }

        public async Task SendPushNotificationAsync(Guid userId, string title, string body, string? dataPayload = null)
        {
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Title = title,
                Message = body,
                IsRead = false,
                RelatedEntityId = !string.IsNullOrEmpty(dataPayload) && Guid.TryParse(dataPayload, out var parsedId) ? parsedId : null,
                CreatedAt = DateTime.UtcNow
            };

            await _notificationRepository.AddAsync(notification);
            await _notificationRepository.SaveChangesAsync();

            await _hubContext.Clients.User(userId.ToString()).SendAsync("ReceiveNotification", new 
            {
                Id = notification.Id,
                Title = notification.Title,
                Message = notification.Message,
                CreatedAt = notification.CreatedAt,
                IsRead = notification.IsRead,
                RelatedEntityId = notification.RelatedEntityId
            });
        }
    }
}
