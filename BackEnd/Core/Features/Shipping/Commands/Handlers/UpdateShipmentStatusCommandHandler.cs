using System.Threading;
using System.Threading.Tasks;
using Core.Base;
using Core.Features.Shipping.Commands.Models;
using MediatR;
using Service.Services.ShipmentService;

namespace Core.Features.Shipping.Commands.Handlers
{
    public class UpdateShipmentStatusCommandHandler : IRequestHandler<UpdateShipmentStatusCommand, Response<string>>
    {
        private readonly IShipmentService _shipmentService;

        public UpdateShipmentStatusCommandHandler(IShipmentService shipmentService)
        {
            _shipmentService = shipmentService;
        }

        public async Task<Response<string>> Handle(UpdateShipmentStatusCommand request, CancellationToken cancellationToken)
        {
            await _shipmentService.UpdateShipmentStatusAsync(request.ShipmentId, request.NewStatus, request.CurrentUserId);
            return new Response<string>($"Shipment status updated to {request.NewStatus}") { Succeeded = true };
        }
    }
}
