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
    public class GetReceivedOrdersQueryHandler : IRequestHandler<GetReceivedOrdersQuery, PaginatedResult<OrderResponse>>
    {
        private readonly IOrderService _orderService;

        public GetReceivedOrdersQueryHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<PaginatedResult<OrderResponse>> Handle(GetReceivedOrdersQuery request, CancellationToken cancellationToken)
        {
            // 1. جلب البيانات من السيرفيس
            var result = await _orderService.GetPaginatedReceivedOrdersForOrgAsync(
                request.OrganizationId,
                request.PageNumber,
                request.PageSize,
                request.Status);

            // 2. المابينج (Mapping)
            var mappedOrders = result.Orders.Select(o => new OrderResponse
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Status = o.Status.ToString(),
                ShippingFees = o.ShippingFees,
                BuyerId = o.BuyerId,

                // هنا بنفلتر الـ Items عشان نرجع للبائع المنتجات بتاعته هو بس جوه الفاتورة
                Items = o.OrderItems!
                    .Where(i => i.product?.OrganizationId == request.OrganizationId)
                    .Select(i => new OrderItemResponse
                    {
                        ProductId = i.productid,
                        ProductName = i.product?.Name ?? "منتج غير معروف",
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice
                    }).ToList()
            }).ToList();

            // 3. إرجاع النتيجة
            return new PaginatedResult<OrderResponse>(
                mappedOrders,
                result.TotalCount,
                request.PageNumber,
                request.PageSize);
        }
    }
}