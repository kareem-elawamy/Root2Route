using System.Threading;
using System.Threading.Tasks;
using Core.Base;
using Core.Features.Orders.Commands.Models;
using MediatR;
using Service.Services.OrderService;

namespace Core.Features.Orders.Commands.Handlers
{
    public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, Response<string>>
    {
        private readonly IOrderService _orderService;

        public CancelOrderCommandHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<Response<string>> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            var (success, message) = await _orderService.CancelOrderAsync(request.OrderId, request.BuyerId);

            if (!success)
                return new Response<string>(message) { Succeeded = false };

            return new Response<string>(message) { Succeeded = true };
        }
    }
}