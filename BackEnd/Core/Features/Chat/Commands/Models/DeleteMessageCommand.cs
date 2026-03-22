using System;
using MediatR;

namespace Core.Features.Chat.Commands.Models
{
    public class DeleteMessageCommand : IRequest<Response<string>>
    {
        public Guid MessageId { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public Guid CurrentUserId { get; set; }
    }
}
