using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Service.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ILogger<ChatHub> _logger;
        private readonly ApplicationDbContext _dbContext;

        public ChatHub(ILogger<ChatHub> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task JoinRoom(string roomId)
        {
            try
            {
                if (!Guid.TryParse(roomId, out var roomGuid)) throw new HubException("Decoded Room ID malformed.");
                
                var userIdClaim = Context.UserIdentifier;
                if (!Guid.TryParse(userIdClaim, out var userId)) throw new HubException("Unauthorized WebSocket Connection.");

                var room = await _dbContext.ChatRooms
                    .Include(r => r.Organization)
                    .ThenInclude(o => o.Members)
                    .FirstOrDefaultAsync(r => r.Id == roomGuid);

                if (room == null) throw new HubException("Secured Domain Room missing.");

                bool isBuyer = room.BuyerId == userId;
                bool isSellerMember = room.Organization != null && room.Organization.Members.Any(m => m.UserId == userId && m.IsActive);

                if (!isBuyer && !isSellerMember) throw new HubException("Explicitly unauthorized to interface visually with this room pipeline.");

                await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
                _logger.LogInformation("Connection {ConnectionId} joined room {RoomId}", Context.ConnectionId, roomId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error joining ChatRoom {RoomId}", roomId);
                throw new HubException("Failed to join chat room.");
            }
        }

        public async Task LeaveRoom(string roomId)
        {
            try
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
                _logger.LogInformation("Connection {ConnectionId} left room {RoomId}", Context.ConnectionId, roomId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error leaving ChatRoom {RoomId}", roomId);
                throw new HubException("Failed to leave chat room.");
            }
        }

        public async Task SendTypingIndicator(string roomId)
        {
            try
            {
                // Retrieve sender context dynamically. Usually decoded from User Identity Context via JWT.
                var senderId = Context.UserIdentifier;
                
                await Clients.OthersInGroup(roomId).SendAsync("ReceiveTypingIndicator", senderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to broadcast typing indicator in room {RoomId}", roomId);
            }
        }
    }
}
