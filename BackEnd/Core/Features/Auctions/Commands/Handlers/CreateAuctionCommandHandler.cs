using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Core.Features.Auctions.Commands.Models;
using Service.Services.AuctionService;
using Domain.Models;

namespace Core.Features.Auctions.Commands.Handlers
{
    public class CreateAuctionCommandHandler : ResponseHandler, IRequestHandler<CreateAuctionCommand, Response<Guid>>
    {
        private readonly IAuctionService _auctionService;

        public CreateAuctionCommandHandler(IAuctionService auctionService)
        {
            _auctionService = auctionService;
        }

        public async Task<Response<Guid>> Handle(CreateAuctionCommand request, CancellationToken cancellationToken)
        {
            var auction = new Auction
            {
                Title = request.Title,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                StartPrice = request.StartPrice,
                MinimumBidIncrement = request.MinimumBidIncrement,
                ReservePrice = request.ReservePrice,
                ProductId = request.ProductId
            };

            var result = await _auctionService.CreateAuctionAsync(auction);
            return Success(result);
        }
    }
}
