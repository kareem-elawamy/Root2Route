using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Features.Chat.Commands.Models;
using Core.Features.Chat.Queries.DTOs;
using MediatR;
using Service.Services.ChatService;

namespace Core.Features.Chat.Commands.Handlers
{
    public class SendMessageCommandHandler : ResponseHandler, IRequestHandler<SendMessageCommand, Response<ChatMessageResponse>>
    {
        private readonly IChatService _chatService;

        public SendMessageCommandHandler(IChatService chatService)
        {
            _chatService = chatService;
        }

        public async Task<Response<ChatMessageResponse>> Handle(SendMessageCommand request, CancellationToken cancellationToken)
        {
            var message = await _chatService.SendMessageAsync(request.CurrentUserId, request.ChatRoomId, request.Content, request.Type, request.ProposedPrice, request.ProposedQuantity);
            
            var dto = new ChatMessageResponse
            {
                Id = message.Id,
                ChatRoomId = message.ChatRoomId,
                SenderId = message.SenderId,
                Content = message.Content,
                Type = message.Type,
                IsRead = message.IsRead,
                SentAt = message.SentAt,
                ProposedPrice = message.ProposedPrice,
                ProposedQuantity = message.ProposedQuantity,
                RelatedOrderId = message.RelatedOrderId
            };

            return Success(dto);
        }
    }
}
