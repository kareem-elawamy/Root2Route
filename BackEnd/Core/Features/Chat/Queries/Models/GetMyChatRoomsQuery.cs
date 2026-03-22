using MediatR;
using System.Collections.Generic;
using Core.Features.Chat.Queries.DTOs;

namespace Core.Features.Chat.Queries.Models
{
    public class GetMyChatRoomsQuery : IRequest<Response<List<ChatRoomResponse>>>
    {
        [System.Text.Json.Serialization.JsonIgnore]
        public System.Guid CurrentUserId { get; set; }
    }
}
