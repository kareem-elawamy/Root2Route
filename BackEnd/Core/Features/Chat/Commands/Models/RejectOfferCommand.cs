using System;
using MediatR;

namespace Core.Features.Chat.Commands.Models
{
    public class RejectOfferCommand : IRequest<Response<string>>
    {
        public Guid OfferMessageId { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public Guid CurrentUserId { get; set; }
    }
}
