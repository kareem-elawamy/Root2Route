using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Features.Chat.Commands.Models;
using MediatR;
using Service.Services.ChatService;

namespace Core.Features.Chat.Commands.Handlers
{
    public class StartChatCommandHandler : ResponseHandler, IRequestHandler<StartChatCommand, Response<Guid>>
    {
        private readonly IChatService _chatService;

        public StartChatCommandHandler(IChatService chatService)
        {
            _chatService = chatService;
        }

        public async Task<Response<Guid>> Handle(StartChatCommand request, CancellationToken cancellationToken)
        {
            var result = await _chatService.StartChatAsync(request.CurrentUserId, request.OrganizationId, request.ProductId);
            return Success(result);
        }
    }
}
