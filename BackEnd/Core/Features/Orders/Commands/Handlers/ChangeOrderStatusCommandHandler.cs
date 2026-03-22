using System.Threading;
using System.Threading.Tasks;
using Core.Base;
using Core.Features.Orders.Commands.Models;
using MediatR;
using Service.Services.OrderService;

namespace Core.Features.Orders.Commands.Handlers
{
    public class ChangeOrderStatusCommandHandler : IRequestHandler<ChangeOrderStatusCommand, Response<string>>
    {
        private readonly IOrderService _orderService;

        public ChangeOrderStatusCommandHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<Response<string>> Handle(ChangeOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var result = await _orderService.UpdateOrderStatusAsync(
                request.OrderId,
                request.NewStatus,
                request.CurrentUserId,
                request.Note);

            return new Response<string>(result) { Succeeded = true };
        }
    }
}
