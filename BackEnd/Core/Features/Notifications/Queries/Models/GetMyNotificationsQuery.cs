using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Core.Base;
using Core.Features.Notifications.Queries.Results;
using MediatR;

namespace Core.Features.Notifications.Queries.Models
{
    public class GetMyNotificationsQuery : IRequest<Response<List<NotificationResponse>>>
    {
        [JsonIgnore]
        public Guid CurrentUserId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
