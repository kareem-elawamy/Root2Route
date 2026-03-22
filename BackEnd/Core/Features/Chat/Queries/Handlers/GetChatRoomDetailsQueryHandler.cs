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
    public class GetChatRoomDetailsQueryHandler : IRequestHandler<GetChatRoomDetailsQuery, Response<ChatRoomDetailsResponse>>
    {
        private readonly IChatRoomRepository _chatRoomRepository;

        public GetChatRoomDetailsQueryHandler(IChatRoomRepository chatRoomRepository)
        {
            _chatRoomRepository = chatRoomRepository;
        }

        public async Task<Response<ChatRoomDetailsResponse>> Handle(GetChatRoomDetailsQuery request, CancellationToken cancellationToken)
        {
            var room = await _chatRoomRepository.GetTableNoTracking()
                .Include(r => r.Organization)
                .ThenInclude(o => o.Members)
                .Include(r => r.Product)
                .FirstOrDefaultAsync(r => r.Id == request.ChatRoomId, cancellationToken);

            if (room == null)
                return new Response<ChatRoomDetailsResponse> { Succeeded = false, Message = "Chat room not found." };

            bool isBuyer = room.BuyerId == request.CurrentUserId;
            bool isSellerMember = room.Organization != null && room.Organization.Members.Any(m => m.UserId == request.CurrentUserId && m.IsActive);

            if (!isBuyer && !isSellerMember)
                return new Response<ChatRoomDetailsResponse> { Succeeded = false, Message = "You are not a participant of this chat room." };

            var dto = new ChatRoomDetailsResponse
            {
                OrganizationName = room.Organization?.Name ?? "Unknown Organization",
                OrganizationLogo = room.Organization?.LogoUrl,
                ProductName = room.Product?.Name,
                ProductOriginalPrice = room.Product?.DirectSalePrice,
                IsProductChat = room.ProductId.HasValue,
                IsClosed = room.IsClosed
            };

            return new Response<ChatRoomDetailsResponse> { Succeeded = true, Data = dto };
        }
    }
}
