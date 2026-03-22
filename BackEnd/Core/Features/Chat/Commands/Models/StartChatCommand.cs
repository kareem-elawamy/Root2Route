using MediatR;
using System;
using Core.Base; // Or corresponding Response generic namespace normally used

namespace Core.Features.Chat.Commands.Models
{
    public class StartChatCommand : IRequest<Response<Guid>>
    {
        public Guid OrganizationId { get; set; }
        public Guid? ProductId { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public Guid CurrentUserId { get; set; }
    }
}
