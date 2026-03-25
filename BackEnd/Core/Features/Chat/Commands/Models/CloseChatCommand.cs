using System;
using MediatR;

namespace Core.Features.Chat.Commands.Models
{
    public class CloseChatCommand : IRequest<Response<string>>
    {
        public Guid ChatRoomId { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public Guid CurrentUserId { get; set; }
    }
}
