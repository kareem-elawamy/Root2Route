using System.Threading;
using System.Threading.Tasks;
using Core.Base;
using Core.Features.Orders.Commands.Models;
using MediatR;
using Service.Services.OrderService;
using Domain.Enums;

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
            var result = await _orderService.UpdateOrderStatusAsync(
                request.OrderId,
                OrderStatus.Cancelled,
                request.BuyerId,
                "Cancelled by user");

            return new Response<string>(result) { Succeeded = true };
        }
    }
}