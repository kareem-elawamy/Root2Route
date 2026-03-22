using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Base;
using Core.Features.Orders.Queries.Models;
using Core.Features.Orders.Queries.Results;
using Infrastructure.Repositories.OrderRepository;
using Infrastructure.Repositories.OrderStatusHistoryRepository;
using Infrastructure.Repositories.OrganizationMemberRepository;
using Infrastructure.Repositories.PaymentRepository;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Features.Orders.Queries.Handlers
{
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, Response<OrderResponse>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderStatusHistoryRepository _orderStatusHistoryRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IOrganizationMemberRepository _organizationMemberRepository;

        public GetOrderByIdQueryHandler(
            IOrderRepository orderRepository,
            IOrderStatusHistoryRepository orderStatusHistoryRepository,
            IPaymentRepository paymentRepository,
            IOrganizationMemberRepository organizationMemberRepository)
        {
            _orderRepository = orderRepository;
            _orderStatusHistoryRepository = orderStatusHistoryRepository;
            _paymentRepository = paymentRepository;
            _organizationMemberRepository = organizationMemberRepository;
        }

        public async Task<Response<OrderResponse>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetTableNoTracking()
                .Include(o => o.OrderItems!)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Organization)
                .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

            if (order == null)
                return new Response<OrderResponse>("Order not found.") { Succeeded = false };

            // Authorization: must be buyer OR member of the selling organization
            var isBuyer = order.BuyerId == request.UserId;
            var isSeller = await _organizationMemberRepository.GetTableNoTracking()
                .AnyAsync(m => m.OrganizationId == order.OrganizationId && m.UserId == request.UserId && m.IsActive, cancellationToken);

            if (!isBuyer && !isSeller)
                return new Response<OrderResponse>("You are not authorized to view this order.")
                    { Succeeded = false, StatusCode = System.Net.HttpStatusCode.Unauthorized };

            var statusHistory = await _orderStatusHistoryRepository.GetTableNoTracking()
                .Where(h => h.OrderId == order.Id)
                .OrderBy(h => h.ChangedAt)
                .Select(h => new OrderStatusHistoryResponse
                {
                    OldStatus = h.OldStatus.ToString(),
                    NewStatus = h.NewStatus.ToString(),
                    ChangedAt = h.ChangedAt,
                    Note = h.Note
                })
                .ToListAsync(cancellationToken);

            var payment = await _paymentRepository.GetTableNoTracking()
                .Where(p => p.OrderId == order.Id)
                .Select(p => new PaymentResponse
                {
                    Id = p.Id,
                    Amount = p.Amount,
                    PaymentMethod = p.PaymentMethod.ToString(),
                    PaymentStatus = p.PaymentStatus.ToString(),
                    PaidAt = p.PaidAt
                })
                .FirstOrDefaultAsync(cancellationToken);

            var mapped = new OrderResponse
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                ShippingFees = order.ShippingFees,
                Status = order.Status.ToString(),
                BuyerId = order.BuyerId,
                OrganizationId = order.OrganizationId,
                OrganizationName = order.Organization?.Name,
                ReceiverName = order.ReceiverName,
                ReceiverPhone = order.ReceiverPhone,
                ShippingCity = order.ShippingCity,
                ShippingStreet = order.ShippingStreet,
                BuildingNumber = order.BuildingNumber,
                PaymentMethod = order.PaymentMethod.ToString(),
                PaymentStatus = order.PaymentStatus.ToString(),
                Items = order.OrderItems!.Select(i => new OrderItemResponse
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product?.Name ?? "Unknown",
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList(),
                StatusHistory = statusHistory,
                Payment = payment
            };

            return new Response<OrderResponse>(mapped) { Succeeded = true };
        }
    }
}
