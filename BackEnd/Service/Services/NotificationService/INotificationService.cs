using System;
using System.Threading.Tasks;

namespace Service.Services.NotificationService
{
    public interface INotificationService
    {
        Task SendPushNotificationAsync(Guid userId, string title, string body, string? dataPayload = null);
    }
}
