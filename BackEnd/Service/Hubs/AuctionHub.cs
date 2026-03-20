using Microsoft.AspNetCore.SignalR;
using Service.Services.AuctionService;
using System.Threading.Tasks;

namespace Service.Hubs
{
    public class AuctionHub : Hub
    {
        private readonly IAuctionService _auctionService;

        public AuctionHub(IAuctionService auctionService)
        {
            _auctionService = auctionService;
        }

        public async Task<AuctionStateResponse> GetAuctionState(Guid auctionId)
        {
            var auction = await _auctionService.GetAuctionByIdAsync(auctionId);
            return new AuctionStateResponse
            {
                CurrentHighestBid = auction?.CurrentHighestBid ?? 0,
                HighestBidderId = auction?.HighestBidderId
            };
        }

        public async Task JoinAuctionGroup(string auctionId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, auctionId);
        }

        public async Task LeaveAuctionGroup(string auctionId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, auctionId);
        }
    }

    public class AuctionStateResponse
    {
        public decimal CurrentHighestBid { get; set; }
        public Guid? HighestBidderId { get; set; }
    }
}
