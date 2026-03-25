using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Models;

namespace Service.Services.NotificationService
{
    public interface INotificationService
    {
        Task SendPushNotificationAsync(Guid userId, string title, string body, string? dataPayload = null);
        Task<(List<Notification> Notifications, int TotalCount)> GetMyNotificationsAsync(Guid userId, int pageNumber, int pageSize);
        Task<int> GetUnreadCountAsync(Guid userId);
        Task MarkAsReadAsync(Guid notificationId, Guid userId);
        Task MarkAllAsReadAsync(Guid userId);
    }
}
