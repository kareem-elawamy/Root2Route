using MediatR;
using System;
using System.Collections.Generic;
using Core.Features.Chat.Queries.DTOs;

namespace Core.Features.Chat.Queries.Models
{
    public class GetChatHistoryQuery : IRequest<Response<List<ChatMessageResponse>>>
    {
        public Guid ChatRoomId { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;

        [System.Text.Json.Serialization.JsonIgnore]
        public Guid CurrentUserId { get; set; }
    }
}
