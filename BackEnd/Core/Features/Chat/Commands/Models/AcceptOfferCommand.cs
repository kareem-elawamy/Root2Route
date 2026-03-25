using MediatR;
using System;

namespace Core.Features.Chat.Commands.Models
{
    public class AcceptOfferCommand : IRequest<Response<Guid>>
    {
        public Guid OfferMessageId { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public Guid CurrentUserId { get; set; }
    }
}
