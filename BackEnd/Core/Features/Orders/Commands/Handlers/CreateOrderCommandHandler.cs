using Core.Base;
using Core.Features.Orders.Commands.Models;
using MediatR;
using Service.DTOs;
using Service.Services.OrderService;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Features.Orders.Commands.Handlers
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Response<string>>
    {
        private readonly IOrderService _orderService;

        public CreateOrderCommandHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<Response<string>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var createOrderDto = new CreateOrderDto
            {
                BuyerId = request.BuyerId,
                ReceiverName = request.ReceiverName,
                ReceiverPhone = request.ReceiverPhone,
                ShippingCity = request.ShippingCity,
                ShippingStreet = request.ShippingStreet,
                BuildingNumber = request.BuildingNumber,
                Items = request.Items.Select(i => new OrderItemDto
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                }).ToList()
            };

            var (order, message) = await _orderService.CreateOrderAsync(createOrderDto);

            if (order == null)
            {
                return new Response<string>(message) { Succeeded = false };
            }

            return new Response<string>(message) { Succeeded = true };
        }
    }
}