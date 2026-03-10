using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Base;
using Core.Features.Orders.Queries.Models;
using Core.Features.Orders.Queries.Results;
using MediatR;
using Service.Services.OrderService;

namespace Core.Features.Orders.Queries.Handlers
{
    public class GetMyOrdersQueryHandler : IRequestHandler<GetMyOrdersQuery, Response<List<OrderResponse>>>
    {
        private readonly IOrderService _orderService;
        public GetMyOrdersQueryHandler(IOrderService orderService) { _orderService = orderService; }

        public async Task<Response<List<OrderResponse>>> Handle(GetMyOrdersQuery request, CancellationToken cancellationToken)
        {
            var orders = await _orderService.GetOrdersByBuyerIdAsync(request.BuyerId);

            var mappedOrders = orders.Select(o => new OrderResponse
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Status = o.Status.ToString(),
                BuyerId = o.BuyerId,
                Items = o.OrderItems!.Select(i => new OrderItemResponse
                {
                    ProductId = i.productid,
                    ProductName = i.product?.Name ?? "منتج غير معروف",
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            }).ToList();

            return new Response<List<OrderResponse>>(mappedOrders) { Succeeded = true };
        }
    }
}