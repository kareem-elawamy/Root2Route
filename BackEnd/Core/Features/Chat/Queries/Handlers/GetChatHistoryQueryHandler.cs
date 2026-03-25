using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Features.Chat.Queries.Models;
using Core.Features.Chat.Queries.DTOs;
using MediatR;
using Infrastructure.Repositories.ChatMessageRepository;
using Infrastructure.Repositories.ChatRoomRepository;
using Microsoft.EntityFrameworkCore;
using System;

namespace Core.Features.Chat.Queries.Handlers
{
    public class GetChatHistoryQueryHandler : ResponseHandler, IRequestHandler<GetChatHistoryQuery, Response<List<ChatMessageResponse>>>
    {
        private readonly IChatMessageRepository _chatMessageRepository;
        private readonly IChatRoomRepository _chatRoomRepository;

        public GetChatHistoryQueryHandler(IChatMessageRepository chatMessageRepository, IChatRoomRepository chatRoomRepository)
        {
            _chatMessageRepository = chatMessageRepository;
            _chatRoomRepository = chatRoomRepository;
        }

        public async Task<Response<List<ChatMessageResponse>>> Handle(GetChatHistoryQuery request, CancellationToken cancellationToken)
        {
            var room = await _chatRoomRepository.GetTableNoTracking()
                .Include(r => r.Organization)
                .ThenInclude(o => o.Members)
                .FirstOrDefaultAsync(r => r.Id == request.ChatRoomId, cancellationToken);

            if (room == null)
                return NotFound<List<ChatMessageResponse>>("Chat room not found.");

            bool isBuyer = room.BuyerId == request.CurrentUserId;
            bool isSellerMember = room.Organization != null && room.Organization.Members.Any(m => m.UserId == request.CurrentUserId && m.IsActive);
            
            if (!isBuyer && !isSellerMember)
                return Unauthorized<List<ChatMessageResponse>>("You are not a participant of this chat room.");

            var pagedMessages = await _chatMessageRepository.GetTableNoTracking()
                .Where(m => m.ChatRoomId == request.ChatRoomId)
                .OrderByDescending(m => m.SentAt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(m => new ChatMessageResponse
                {
                    Id = m.Id,
                    ChatRoomId = m.ChatRoomId,
                    SenderId = m.SenderId,
                    Content = m.Content,
                    Type = m.Type,
                    IsRead = m.IsRead,
                    SentAt = m.SentAt,
                    ProposedPrice = m.ProposedPrice,
                    ProposedQuantity = m.ProposedQuantity,
                    RelatedOrderId = m.RelatedOrderId
                })
                .ToListAsync(cancellationToken);

            var chronologicalMessages = pagedMessages.OrderBy(m => m.SentAt).ToList();
            return Success(chronologicalMessages);
        }
    }
}
