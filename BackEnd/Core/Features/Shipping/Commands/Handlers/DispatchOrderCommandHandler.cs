using System.Threading;
using System.Threading.Tasks;
using Core.Base;
using Core.Features.Shipping.Commands.Models;
using MediatR;
using Service.Services.ShipmentService;

namespace Core.Features.Shipping.Commands.Handlers
{
    public class DispatchOrderCommandHandler : IRequestHandler<DispatchOrderCommand, Response<string>>
    {
        private readonly IShipmentService _shipmentService;

        public DispatchOrderCommandHandler(IShipmentService shipmentService)
        {
            _shipmentService = shipmentService;
        }

        public async Task<Response<string>> Handle(DispatchOrderCommand request, CancellationToken cancellationToken)
        {
            var shipment = await _shipmentService.DispatchOrderAsync(
                request.OrderId,
                request.CarrierName,
                request.TrackingNumber,
                request.DriverPhone,
                request.CurrentUserId);

            return new Response<string>($"Order dispatched. Tracking: {shipment.TrackingNumber}") { Succeeded = true };
        }
    }
}
