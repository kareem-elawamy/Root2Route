using Microsoft.AspNetCore.SignalR;
using Service.Services.AuctionService;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Service.Hubs
{
    public class AuctionHub : Hub
    {
        private readonly IAuctionService _auctionService;
        private readonly ILogger<AuctionHub> _logger;

        public AuctionHub(IAuctionService auctionService, ILogger<AuctionHub> logger)
        {
            _auctionService = auctionService;
            _logger = logger;
        }

        public async Task<AuctionStateResponse> GetAuctionState(Guid auctionId)
        {
            try
            {
                var auction = await _auctionService.GetAuctionByIdAsync(auctionId);
                return new AuctionStateResponse
                {
                    CurrentHighestBid = auction?.CurrentHighestBid ?? 0,
                    HighestBidderId = auction?.HighestBidderId
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching auction state for {AuctionId}", auctionId);
                return new AuctionStateResponse();
            }
        }

        public async Task JoinAuctionGroup(string auctionId)
        {
            try
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, auctionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error joining group for {AuctionId}", auctionId);
                throw new HubException("Failed to join auction group.");
            }
        }

        public async Task LeaveAuctionGroup(string auctionId)
        {
            try
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, auctionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error leaving group for {AuctionId}", auctionId);
                throw new HubException("Failed to leave auction group.");
            }
        }
    }

    public class AuctionStateResponse
    {
        public decimal CurrentHighestBid { get; set; }
        public Guid? HighestBidderId { get; set; }
    }
}
