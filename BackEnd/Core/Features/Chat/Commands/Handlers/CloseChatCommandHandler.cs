using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Features.Chat.Commands.Models;
using MediatR;
using Service.Services.ChatService;

namespace Core.Features.Chat.Commands.Handlers
{
    public class CloseChatCommandHandler : IRequestHandler<CloseChatCommand, Response<string>>
    {
        private readonly IChatService _chatService;

        public CloseChatCommandHandler(IChatService chatService)
        {
            _chatService = chatService;
        }

        public async Task<Response<string>> Handle(CloseChatCommand request, CancellationToken cancellationToken)
        {
            var result = await _chatService.CloseChatAsync(request.CurrentUserId, request.ChatRoomId);
            return new Response<string> { Succeeded = true, Data = result };
        }
    }
}
