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
            var result = await _orderService.GetPaginatedReceivedOrdersForOrgAsync(
                request.OrganizationId,
                request.PageNumber,
                request.PageSize,
                request.Status);

            var mappedOrders = result.Orders.Select(o => new OrderResponse
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Status = o.Status.ToString(),
                ShippingFees = o.ShippingFees,
                BuyerId = o.BuyerId,
                OrganizationId = o.OrganizationId,
                OrganizationName = o.Organization?.Name,

                Items = o.OrderItems!
                    .Where(i => i.Product?.OrganizationId == request.OrganizationId)
                    .Select(i => new OrderItemResponse
                    {
                        ProductId = i.ProductId,
                        ProductName = i.Product?.Name ?? "Unknown",
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice
                    }).ToList()
            }).ToList();

            return new PaginatedResult<OrderResponse>(
                mappedOrders,
                result.TotalCount,
                request.PageNumber,
                request.PageSize);
        }
    }
}
