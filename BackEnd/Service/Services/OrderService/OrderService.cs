using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enums;
using Domain.Models;
using Infrastructure.Repositories.OrderRepository;
using Infrastructure.Repositories.ProductRepository;
using Microsoft.EntityFrameworkCore;
using Service.DTOs;

namespace Service.Services.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;

        public OrderService(IOrderRepository orderRepository, IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }

        public async Task<(Order? Order, string Message)> CreateOrderAsync(CreateOrderDto orderDto)
        {
            var productIds = orderDto.Items.Select(i => i.ProductId).ToList();

            var products = await _productRepository
                .GetTableNoTracking()
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync();

            if (products.Count != productIds.Count)
                return (null, "عفواً، بعض المنتجات المطلوبة غير موجودة في النظام");

            var orderItems = new List<OrderItem>();
            decimal totalAmount = 0;

            foreach (var product in products)
            {
                var requestedItem = orderDto.Items.First(i => i.ProductId == product.Id);
                var requestedQuantity = requestedItem.Quantity;

                if (product.Status != ProductStatus.Approved || !product.IsAvailableForDirectSale)
                    return (null, $"المنتج '{product.Name}' غير متاح للبيع المباشر حالياً");

                // 1. Atomic decrement in Database resolving Race Conditions
                var rowsAffected = await _productRepository
                    .GetTableNoTracking()
                    .Where(p => p.Id == product.Id && p.StockQuantity >= requestedQuantity)
                    .ExecuteUpdateAsync(s =>
                        s.SetProperty(
                            p => p.StockQuantity,
                            p => p.StockQuantity - requestedQuantity
                        )
                    );

                if (rowsAffected == 0)
                    return (
                        null,
                        $"الكمية المطلوبة من '{product.Name}' تتجاوز المخزون المتاح أو نفدت للتو"
                    );

                var itemPrice = product.DirectSalePrice;
                totalAmount += itemPrice * requestedQuantity;

                orderItems.Add(
                    new OrderItem
                    {
                        productid = product.Id,
                        Quantity = requestedQuantity,
                        UnitPrice = itemPrice,
                    }
                );
            }

            var newOrder = new Order
            {
                BuyerId = orderDto.BuyerId,
                OrderDate = DateTime.UtcNow,
                TotalAmount = totalAmount,
                Status = OrderStatus.Pending,
                OrderItems = orderItems,
                ReceiverName = orderDto.ReceiverName,
                ReceiverPhone = orderDto.ReceiverPhone,
                ShippingCity = orderDto.ShippingCity,
                ShippingStreet = orderDto.ShippingStreet,
                BuildingNumber = orderDto.BuildingNumber,
            };

            await _orderRepository.AddAsync(newOrder);

            return (newOrder, "تم إنشاء الطلب بنجاح");
        }

        public async Task<Order?> GetOrderByIdAsync(Guid orderId)
        {
            return await _orderRepository
                .GetTableNoTracking()
                .Include(o => o.OrderItems!)
                    .ThenInclude(oi => oi.product)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public IQueryable<Order> GetOrdersByBuyerIdQueryable(Guid buyerId)
        {
            // 4. Returns IQueryable without execution, stopping domain leak memory issues
            return _orderRepository
                .GetTableNoTracking()
                .Where(o => o.BuyerId == buyerId)
                .OrderByDescending(o => o.OrderDate);
        }

        public async Task<(
            List<Order> Orders,
            int TotalCount
        )> GetPaginatedReceivedOrdersForOrgAsync(
            Guid organizationId,
            int pageNumber,
            int pageSize,
            OrderStatus? status = null
        )
        {
            var query = _orderRepository
                .GetTableNoTracking()
                .Include(o => o.OrderItems!)
                    .ThenInclude(oi => oi.product)
                .Where(o => o.OrderItems!.Any(oi => oi.product!.OrganizationId == organizationId))
                .AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(o => o.Status == status.Value);
            }

            var totalCount = await query.CountAsync();

            var orders = await query
                .OrderByDescending(o => o.OrderDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (orders, totalCount);
        }

        public async Task<bool> ChangeOrderStatusAsync(
            Guid orderId,
            OrderStatus newStatus,
            decimal shippingFees = 0
        )
        {
            // 3. Explicitly reject negative fees
            if (shippingFees < 0)
            {
                throw new ArgumentException("رسوم الشحن لا يمكن أن تكون أقل من الصفر");
            }

            var order = await _orderRepository
                .GetTableAsTracking()
                .Include(o => o.OrderItems!)
                    .ThenInclude(oi => oi.product)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return false;

            if (newStatus == OrderStatus.Cancelled && order.Status != OrderStatus.Cancelled)
            {
                foreach (var item in order.OrderItems!)
                {
                    if (item.product != null)
                    {
                        item.product.StockQuantity += item.Quantity;
                    }
                }
            }

            order.Status = newStatus;
            order.ShippingFees = shippingFees; // Don't forget to assign it!

            await _orderRepository.UpdateAsync(order);

            return true;
        }

        public async Task<(bool Success, string Message)> CancelOrderAsync(
            Guid orderId,
            Guid buyerId
        )
        {
            var order = await _orderRepository
                .GetTableAsTracking()
                .Include(o => o.OrderItems!)
                    .ThenInclude(oi => oi.product)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return (false, "الطلب غير موجود");

            if (order.BuyerId != buyerId)
                return (false, "غير مصرح لك بإلغاء هذا الطلب");

            if (order.Status != OrderStatus.Pending)
                return (false, "لا يمكن إلغاء الطلب في هذه المرحلة، تواصل مع البائع");

            foreach (var item in order.OrderItems!)
            {
                if (item.product != null)
                {
                    item.product.StockQuantity += item.Quantity;
                }
            }

            order.Status = OrderStatus.Cancelled;

            await _orderRepository.UpdateAsync(order);

            return (true, "تم إلغاء الطلب بنجاح واسترجاع الكميات للمخزون");
        }
    }
}
