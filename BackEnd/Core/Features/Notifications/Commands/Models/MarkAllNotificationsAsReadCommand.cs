using System;
using System.Text.Json.Serialization;
using Core.Base;
using MediatR;

namespace Core.Features.Notifications.Commands.Models
{
    public class MarkAllNotificationsAsReadCommand : IRequest<Response<string>>
    {
        [JsonIgnore]
        public Guid CurrentUserId { get; set; }
    }
}
