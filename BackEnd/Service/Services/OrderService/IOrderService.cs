using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Enums;
using Domain.Models;

namespace Service.Services.OrderService
{
    public interface IOrderService
    {
        Task<(Order? Order, string Message)> CreateOrderAsync(Guid buyerId, Dictionary<Guid, int> productQuantities);

        Task<Order?> GetOrderByIdAsync(Guid orderId);

        Task<List<Order>> GetOrdersByBuyerIdAsync(Guid buyerId);
        Task<(List<Order> Orders, int TotalCount)> GetPaginatedReceivedOrdersForOrgAsync(Guid organizationId, int pageNumber, int pageSize, OrderStatus? status = null);
        Task<bool> ChangeOrderStatusAsync(Guid orderId, OrderStatus newStatus);
    }
}
