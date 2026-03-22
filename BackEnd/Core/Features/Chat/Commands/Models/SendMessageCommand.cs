using Domain.Enums;
using MediatR;
using System;
using Core.Base;
using Core.Features.Chat.Queries.DTOs;

namespace Core.Features.Chat.Commands.Models
{
    public class SendMessageCommand : IRequest<Response<ChatMessageResponse>> 
    {
        public Guid ChatRoomId { get; set; }
        public string Content { get; set; } = string.Empty;
        public MessageType Type { get; set; }
        public decimal? ProposedPrice { get; set; }
        public int? ProposedQuantity { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public Guid CurrentUserId { get; set; }
    }
}
