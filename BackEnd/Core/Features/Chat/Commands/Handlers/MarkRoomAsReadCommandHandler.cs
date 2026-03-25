using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Features.Chat.Commands.Models;
using MediatR;
using Service.Services.ChatService;

namespace Core.Features.Chat.Commands.Handlers
{
    public class MarkRoomAsReadCommandHandler : ResponseHandler, IRequestHandler<MarkRoomAsReadCommand, Response<string>>
    {
        private readonly IChatService _chatService;

        public MarkRoomAsReadCommandHandler(IChatService chatService)
        {
            _chatService = chatService;
        }

        public async Task<Response<string>> Handle(MarkRoomAsReadCommand request, CancellationToken cancellationToken)
        {
            var result = await _chatService.MarkRoomAsReadAsync(request.CurrentUserId, request.ChatRoomId);
            return Success(result);
        }
    }
}
