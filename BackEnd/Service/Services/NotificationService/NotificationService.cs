using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Infrastructure.Repositories.NotificationRepository;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
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

        public async Task<(List<Notification> Notifications, int TotalCount)> GetMyNotificationsAsync(Guid userId, int pageNumber, int pageSize)
        {
            var query = _notificationRepository.GetTableNoTracking()
                .Where(n => n.UserId == userId);

            var totalCount = await query.CountAsync();

            var notifications = await query
                .OrderByDescending(n => n.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (notifications, totalCount);
        }

        public async Task<int> GetUnreadCountAsync(Guid userId)
        {
            return await _notificationRepository.GetTableNoTracking()
                .CountAsync(n => n.UserId == userId && !n.IsRead);
        }

        public async Task MarkAsReadAsync(Guid notificationId, Guid userId)
        {
            var notification = await _notificationRepository.GetTableAsTracking()
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

            if (notification == null)
                throw new KeyNotFoundException("Notification not found.");

            notification.IsRead = true;
            await _notificationRepository.UpdateAsync(notification);
            await _notificationRepository.SaveChangesAsync();
        }

        public async Task MarkAllAsReadAsync(Guid userId)
        {
            var unreadNotifications = await _notificationRepository.GetTableAsTracking()
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
            }

            if (unreadNotifications.Any())
            {
                await _notificationRepository.UpdateRangeAsync(unreadNotifications);
                await _notificationRepository.SaveChangesAsync();
            }
        }
    }
}
