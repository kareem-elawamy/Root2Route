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
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, Response<OrderResponse>>
    {
        private readonly IOrderService _orderService;
        public GetOrderByIdQueryHandler(IOrderService orderService) { _orderService = orderService; }

        public async Task<Response<OrderResponse>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var order = await _orderService.GetOrderByIdAsync(request.OrderId);

            if (order == null)
                return new Response<OrderResponse>("الطلب غير موجود") { Succeeded = false };
            if (order.BuyerId != request.UserId)
                return new Response<OrderResponse>("غير مصرح لك بعرض تفاصيل هذا الطلب") 
                { Succeeded = false, StatusCode = System.Net.HttpStatusCode.Unauthorized };
            var mappedOrder = new OrderResponse
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                ShippingFees = order.ShippingFees,
                Status = order.Status.ToString(),
                BuyerId = order.BuyerId,
                Items = order.OrderItems!.Select(i => new OrderItemResponse
                {
                    ProductId = i.productid,
                    ProductName = i.product?.Name ?? "منتج غير معروف",
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };

            return new Response<OrderResponse>(mappedOrder) { Succeeded = true };
        }
    }
}