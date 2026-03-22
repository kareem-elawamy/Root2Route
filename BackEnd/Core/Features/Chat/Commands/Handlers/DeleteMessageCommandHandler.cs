using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Features.Chat.Commands.Models;
using MediatR;
using Service.Services.ChatService;

namespace Core.Features.Chat.Commands.Handlers
{
    public class DeleteMessageCommandHandler : IRequestHandler<DeleteMessageCommand, Response<string>>
    {
        private readonly IChatService _chatService;

        public DeleteMessageCommandHandler(IChatService chatService)
        {
            _chatService = chatService;
        }

        public async Task<Response<string>> Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
        {
            var result = await _chatService.DeleteMessageAsync(request.CurrentUserId, request.MessageId);
            return new Response<string> { Succeeded = true, Data = result };
        }
    }
}
