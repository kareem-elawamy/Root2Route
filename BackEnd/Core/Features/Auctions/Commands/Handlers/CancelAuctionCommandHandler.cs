using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Core.Features.Auctions.Commands.Models;
using Service.Services.AuctionService;

namespace Core.Features.Auctions.Commands.Handlers
{
    public class CancelAuctionCommandHandler : ResponseHandler, IRequestHandler<CancelAuctionCommand, Response<string>>
    {
        private readonly IAuctionService _auctionService;

        public CancelAuctionCommandHandler(IAuctionService auctionService)
        {
            _auctionService = auctionService;
        }

        public async Task<Response<string>> Handle(CancelAuctionCommand request, CancellationToken cancellationToken)
        {
            var result = await _auctionService.CancelAuctionAsync(request.AuctionId, request.SellerId);

            return result switch
            {
                "Success" => Success("Auction cancelled successfully"),
                "Not Found" => NotFound<string>("Auction not found"),
                "Unauthorized" => Unauthorized<string>("Only the auction owner can cancel it"),
                _ => BadRequest<string>(result)
            };
        }
    }
}
