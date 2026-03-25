using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Features.Chat.Commands.Models;
using MediatR;
using Service.Services.ChatService;

namespace Core.Features.Chat.Commands.Handlers
{
    public class AcceptOfferCommandHandler : ResponseHandler, IRequestHandler<AcceptOfferCommand, Response<Guid>>
    {
        private readonly IChatService _chatService;

        public AcceptOfferCommandHandler(IChatService chatService)
        {
            _chatService = chatService;
        }

        public async Task<Response<Guid>> Handle(AcceptOfferCommand request, CancellationToken cancellationToken)
        {
            var result = await _chatService.AcceptOfferAsync(request.CurrentUserId, request.OfferMessageId);
            return Success(result);
        }
    }
}
