using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Features.Chat.Queries.Models;
using Core.Features.Chat.Queries.DTOs;
using MediatR;
using Infrastructure.Repositories.ChatRoomRepository;
using Microsoft.EntityFrameworkCore;

namespace Core.Features.Chat.Queries.Handlers
{
    public class GetMyChatRoomsQueryHandler : ResponseHandler, IRequestHandler<GetMyChatRoomsQuery, Response<List<ChatRoomResponse>>>
    {
        private readonly IChatRoomRepository _chatRoomRepository;

        public GetMyChatRoomsQueryHandler(IChatRoomRepository chatRoomRepository)
        {
            _chatRoomRepository = chatRoomRepository;
        }

        public async Task<Response<List<ChatRoomResponse>>> Handle(GetMyChatRoomsQuery request, CancellationToken cancellationToken)
        {
            var rooms = await _chatRoomRepository.GetTableNoTracking()
                .Include(r => r.Organization)
                .Include(r => r.Product)
                .Include(r => r.Messages)
                .Where(r => r.BuyerId == request.CurrentUserId || 
                            (r.Organization != null && r.Organization.Members.Any(m => m.UserId == request.CurrentUserId && m.IsActive)))
                .Select(r => new ChatRoomResponse
                {
                    Id = r.Id,
                    BuyerId = r.BuyerId,
                    OrganizationId = r.OrganizationId,
                    OrganizationName = r.Organization != null ? r.Organization.Name : "Unknown Organization",
                    ProductName = r.Product != null ? r.Product.Name : null,
                    LastMessageAt = r.LastMessageAt,
                    LastMessageSnippet = r.Messages.OrderByDescending(m => m.SentAt).Select(m => m.Content).FirstOrDefault() ?? "",
                    UnreadCount = r.Messages.Count(m => !m.IsRead && m.SenderId != request.CurrentUserId)
                })
                .ToListAsync(cancellationToken);

            return Success(rooms.OrderByDescending(r => r.LastMessageAt).ToList());
        }
    }
}
