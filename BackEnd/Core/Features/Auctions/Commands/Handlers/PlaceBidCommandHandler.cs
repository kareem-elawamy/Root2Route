using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Core.Features.Auctions.Commands.Models;
using Service.Services.AuctionService;

using Microsoft.AspNetCore.SignalR;
using Service.Hubs;

namespace Core.Features.Auctions.Commands.Handlers
{
    public class PlaceBidCommandHandler : ResponseHandler, IRequestHandler<PlaceBidCommand, Response<string>>
    {
        private readonly IAuctionService _auctionService;
        private readonly IHubContext<AuctionHub> _hubContext;

        public PlaceBidCommandHandler(IAuctionService auctionService, IHubContext<AuctionHub> hubContext)
        {
            _auctionService = auctionService;
            _hubContext = hubContext;
        }

        public async Task<Response<string>> Handle(PlaceBidCommand request, CancellationToken cancellationToken)
        {
            await _auctionService.PlaceBidAsync(request.AuctionId, request.BidderId, request.Amount);
            
            await _hubContext.Clients.Group(request.AuctionId.ToString())
                .SendAsync("ReceiveNewBid", request.Amount, request.BidderId, cancellationToken);
            
            return Success("Bid placed successfully");
        }
    }
}
