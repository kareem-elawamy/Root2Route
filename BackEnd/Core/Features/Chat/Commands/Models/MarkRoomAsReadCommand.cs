using MediatR;
using System;

namespace Core.Features.Chat.Commands.Models
{
    public class MarkRoomAsReadCommand : IRequest<Response<string>>
    {
        public Guid ChatRoomId { get; set; }
        
        [System.Text.Json.Serialization.JsonIgnore]
        public Guid CurrentUserId { get; set; }
    }
}
