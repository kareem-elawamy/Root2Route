using API.Controllers.Shared;
using Core.Features.Chat.Commands.Models;
using Core.Features.Chat.Queries.Models;
using Domain.MetaData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [Authorize]
    public class ChatController : BaseApiController
    {
        private Guid GetUserId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(idClaim, out var userId) ? userId : Guid.Empty;
        }

        [HttpPost(Router.Chat.StartChat)]
        public async Task<IActionResult> StartChat([FromBody] StartChatCommand command)
        {
            command.CurrentUserId = GetUserId();
            var response = await Mediator.Send(command);
            return NewResult(response);
        }

        [HttpPost(Router.Chat.SendMessage)]
        public async Task<IActionResult> SendMessage([FromForm] SendMessageCommand command)
        {
            command.CurrentUserId = GetUserId();
            var response = await Mediator.Send(command);
            return NewResult(response);
        }

        [HttpPost(Router.Chat.AcceptOffer)]
        public async Task<IActionResult> AcceptOffer([FromBody] AcceptOfferCommand command)
        {
            command.CurrentUserId = GetUserId();
            var response = await Mediator.Send(command);
            return NewResult(response);
        }

        [HttpPost(Router.Chat.RejectOffer)]
        public async Task<IActionResult> RejectOffer([FromBody] RejectOfferCommand command)
        {
            command.CurrentUserId = GetUserId();
            var response = await Mediator.Send(command);
            return NewResult(response);
        }

        [HttpGet(Router.Chat.GetMyRooms)]
        public async Task<IActionResult> GetMyRooms([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            var query = new GetMyChatRoomsQuery { CurrentUserId = GetUserId(), PageNumber = pageNumber, PageSize = pageSize };
            var response = await Mediator.Send(query);
            return NewResult(response);
        }

        [HttpGet(Router.Chat.GetHistory)]
        public async Task<IActionResult> GetHistory([FromRoute] Guid chatRoomId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50)
        {
            var query = new GetChatHistoryQuery { ChatRoomId = chatRoomId, CurrentUserId = GetUserId(), PageNumber = pageNumber, PageSize = pageSize };
            var response = await Mediator.Send(query);
            return NewResult(response);
        }

        [HttpGet(Router.Chat.GetRoomDetails)]
        public async Task<IActionResult> GetRoomDetails([FromRoute] Guid roomId)
        {
            var query = new GetChatRoomDetailsQuery { ChatRoomId = roomId, CurrentUserId = GetUserId() };
            var response = await Mediator.Send(query);
            return NewResult(response);
        }

        [HttpPut(Router.Chat.Prefix + "/{roomId}/read")]
        public async Task<IActionResult> MarkRoomAsRead([FromRoute] Guid roomId)
        {
            var command = new MarkRoomAsReadCommand { ChatRoomId = roomId, CurrentUserId = GetUserId() };
            var response = await Mediator.Send(command);
            return NewResult(response);
        }

        [HttpPut(Router.Chat.CloseChat)]
        public async Task<IActionResult> CloseChat([FromRoute] Guid roomId)
        {
            var command = new CloseChatCommand { ChatRoomId = roomId, CurrentUserId = GetUserId() };
            var response = await Mediator.Send(command);
            return NewResult(response);
        }

        [HttpDelete(Router.Chat.DeleteMessage)]
        public async Task<IActionResult> DeleteMessage([FromRoute] Guid messageId)
        {
            var command = new DeleteMessageCommand { MessageId = messageId, CurrentUserId = GetUserId() };
            var response = await Mediator.Send(command);
            return NewResult(response);
        }
    }
}
