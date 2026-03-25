using System;
using MediatR;
using Core.Features.Chat.Queries.DTOs;

namespace Core.Features.Chat.Queries.Models
{
    public class GetChatRoomDetailsQuery : IRequest<Response<ChatRoomDetailsResponse>>
    {
        public Guid ChatRoomId { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public Guid CurrentUserId { get; set; }
    }
}
