using System;
using System.Text.Json.Serialization;
using Core.Base;
using MediatR;

namespace Core.Features.Notifications.Queries.Models
{
    public class GetUnreadNotificationsCountQuery : IRequest<Response<int>>
    {
        [JsonIgnore]
        public Guid CurrentUserId { get; set; }
    }
}
