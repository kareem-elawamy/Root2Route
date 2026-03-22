using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Base;
using Core.Features.Orders.Queries.Models;
using Core.Features.Orders.Queries.Results;
using Infrastructure.Repositories.OrderRepository;
using Infrastructure.Repositories.OrganizationMemberRepository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Core.Features.Orders.Queries.Handlers
{
    public class GetMyOrdersQueryHandler : IRequestHandler<GetMyOrdersQuery, Response<List<OrderResponse>>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrganizationMemberRepository _organizationMemberRepository;

        public GetMyOrdersQueryHandler(IOrderRepository orderRepository, IOrganizationMemberRepository organizationMemberRepository)
        {
            _orderRepository = orderRepository;
            _organizationMemberRepository = organizationMemberRepository;
        }

        public async Task<Response<List<OrderResponse>>> Handle(GetMyOrdersQuery request, CancellationToken cancellationToken)
        {
            // Get org IDs where the user is an active member (seller side)
            var userOrgIds = await _organizationMemberRepository.GetTableNoTracking()
                .Where(m => m.UserId == request.CurrentUserId && m.IsActive)
                .Select(m => m.OrganizationId)
                .ToListAsync(cancellationToken);

            var orders = await _orderRepository.GetTableNoTracking()
                .Include(o => o.OrderItems!)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Organization)
                .Where(o => o.BuyerId == request.CurrentUserId || userOrgIds.Contains(o.OrganizationId))
                .OrderByDescending(o => o.OrderDate)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var mapped = orders.Select(o => new OrderResponse
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                ShippingFees = o.ShippingFees,
                Status = o.Status.ToString(),
                BuyerId = o.BuyerId,
                OrganizationId = o.OrganizationId,
                OrganizationName = o.Organization?.Name,
                ReceiverName = o.ReceiverName,
                ReceiverPhone = o.ReceiverPhone,
                ShippingCity = o.ShippingCity,
                ShippingStreet = o.ShippingStreet,
                BuildingNumber = o.BuildingNumber,
                PaymentMethod = o.PaymentMethod.ToString(),
                PaymentStatus = o.PaymentStatus.ToString(),
                Items = o.OrderItems!.Select(i => new OrderItemResponse
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product?.Name ?? "Unknown",
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            }).ToList();

            return new Response<List<OrderResponse>>(mapped) { Succeeded = true };
        }
    }
}
