using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Enums;
using Domain.Models;
using Service.DTOs;

namespace Service.Services.OrderService
{
    public interface IOrderService
    {
        Task<(Order? Order, string Message)> CreateOrderAsync(CreateOrderDto orderDto);
        Task<Order?> GetOrderByIdAsync(Guid orderId);
        IQueryable<Order> GetOrdersByBuyerIdQueryable(Guid buyerId);
        Task<(List<Order> Orders, int TotalCount)> GetPaginatedReceivedOrdersForOrgAsync(
            Guid organizationId,
            int pageNumber,
            int pageSize,
            OrderStatus? status = null
        );
        Task<bool> ChangeOrderStatusAsync(
            Guid orderId,
            OrderStatus newStatus,
            decimal shippingFees = 0
        );
        Task<(bool Success, string Message)> CancelOrderAsync(Guid orderId, Guid buyerId);
        Task<string> UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus, Guid currentUserId, string? note = null);
    }
}
