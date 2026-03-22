using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Features.Chat.Commands.Models;
using MediatR;
using Service.Services.ChatService;

namespace Core.Features.Chat.Commands.Handlers
{
    public class RejectOfferCommandHandler : IRequestHandler<RejectOfferCommand, Response<string>>
    {
        private readonly IChatService _chatService;

        public RejectOfferCommandHandler(IChatService chatService)
        {
            _chatService = chatService;
        }

        public async Task<Response<string>> Handle(RejectOfferCommand request, CancellationToken cancellationToken)
        {
            var result = await _chatService.RejectOfferAsync(request.CurrentUserId, request.OfferMessageId);
            return new Response<string> { Succeeded = true, Data = result };
        }
    }
}
