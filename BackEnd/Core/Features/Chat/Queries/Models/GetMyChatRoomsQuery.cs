using MediatR;
using System;
using System.Collections.Generic;
using Core.Features.Chat.Queries.DTOs;

namespace Core.Features.Chat.Queries.Models
{
    public class GetMyChatRoomsQuery : IRequest<Response<List<ChatRoomResponse>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;

        [System.Text.Json.Serialization.JsonIgnore]
        public Guid CurrentUserId { get; set; }
    }
}
