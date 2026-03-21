using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Core.Features.Auctions.Commands.Models;
using Service.Services.AuctionService;

namespace Core.Features.Auctions.Commands.Handlers
{
    public class CheckoutWonAuctionCommandHandler : ResponseHandler, IRequestHandler<CheckoutWonAuctionCommand, Response<string>>
    {
        private readonly IAuctionService _auctionService;

        public CheckoutWonAuctionCommandHandler(IAuctionService auctionService)
        {
            _auctionService = auctionService;
        }

        public async Task<Response<string>> Handle(CheckoutWonAuctionCommand request, CancellationToken cancellationToken)
        {
            var result = await _auctionService.CheckoutWonAuctionAsync(
                request.AuctionId, 
                request.UserId, 
                request.ShippingAddress, 
                request.PaymentMethod);

            if (Guid.TryParse(result, out _))
            {
                return Success($"Order created successfully. OrderId: {result}");
            }

            return result switch
            {
                "Not Found" => NotFound<string>("Auction not found"),
                "Unauthorized: You are not the winner" => Unauthorized<string>(result),
                _ => BadRequest<string>(result)
            };
        }
    }
}
