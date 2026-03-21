using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Core.Features.Auctions.Commands.Models;
using Service.Services.AuctionService;
using Domain.Models;

namespace Core.Features.Auctions.Commands.Handlers
{
    public class UpdateAuctionCommandHandler : ResponseHandler, IRequestHandler<UpdateAuctionCommand, Response<string>>
    {
        private readonly IAuctionService _auctionService;

        public UpdateAuctionCommandHandler(IAuctionService auctionService)
        {
            _auctionService = auctionService;
        }

        public async Task<Response<string>> Handle(UpdateAuctionCommand request, CancellationToken cancellationToken)
        {
            var updatedData = new Auction
            {
                Title = request.Title,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                StartPrice = request.StartPrice,
                MinimumBidIncrement = request.MinimumBidIncrement,
                ReservePrice = request.ReservePrice
            };

            await _auctionService.UpdateAuctionAsync(request.AuctionId, updatedData, request.SellerId);

            return Success("Auction updated successfully");
        }
    }
}
