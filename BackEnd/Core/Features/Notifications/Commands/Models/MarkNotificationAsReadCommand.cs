using System;
using System.Text.Json.Serialization;
using Core.Base;
using MediatR;

namespace Core.Features.Notifications.Commands.Models
{
    public class MarkNotificationAsReadCommand : IRequest<Response<string>>
    {
        public Guid NotificationId { get; set; }

        [JsonIgnore]
        public Guid CurrentUserId { get; set; }
    }
}
