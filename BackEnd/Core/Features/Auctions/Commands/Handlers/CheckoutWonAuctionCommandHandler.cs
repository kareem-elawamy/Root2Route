using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Core.Features.Auctions.Commands.Models;
using Service.Services.AuctionService;

namespace Core.Features.Auctions.Commands.Handlers
{
    public class CheckoutWonAuctionCommandHandler : ResponseHandler, IRequestHandler<CheckoutWonAuctionCommand, Response<Guid>>
    {
        private readonly IAuctionService _auctionService;

        public CheckoutWonAuctionCommandHandler(IAuctionService auctionService)
        {
            _auctionService = auctionService;
        }

        public async Task<Response<Guid>> Handle(CheckoutWonAuctionCommand request, CancellationToken cancellationToken)
        {
            var orderId = await _auctionService.CheckoutWonAuctionAsync(
                request.AuctionId, 
                request.UserId, 
                request.ReceiverName,
                request.ReceiverPhone,
                request.ShippingCity,
                request.ShippingAddress, 
                request.PaymentMethod);

            return Success(orderId);
        }
    }
}
