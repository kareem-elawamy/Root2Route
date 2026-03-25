using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enums;
using Domain.Models;
using Infrastructure.Repositories.OrderRepository;
using Infrastructure.Repositories.OrderStatusHistoryRepository;
using Infrastructure.Repositories.OrganizationMemberRepository;
using Infrastructure.Repositories.PaymentRepository;
using Infrastructure.Repositories.ProductRepository;
using Microsoft.EntityFrameworkCore;
using Service.DTOs;
using Service.Services.NotificationService;

namespace Service.Services.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IOrderStatusHistoryRepository _orderStatusHistoryRepository;
        private readonly IOrganizationMemberRepository _orgMemberRepository;
        private readonly INotificationService _notificationService;

        // Valid state transitions: Key = Current Status, Value = Allowed Next Statuses
        private static readonly Dictionary<OrderStatus, HashSet<OrderStatus>> _validTransitions = new()
        {
            { OrderStatus.Pending, new HashSet<OrderStatus> { OrderStatus.Confirmed, OrderStatus.Cancelled } },
            { OrderStatus.Confirmed, new HashSet<OrderStatus> { OrderStatus.Shipped, OrderStatus.Cancelled } },
            { OrderStatus.Shipped, new HashSet<OrderStatus> { OrderStatus.Delivered } },
            { OrderStatus.Delivered, new HashSet<OrderStatus>() },
            { OrderStatus.Cancelled, new HashSet<OrderStatus>() }
        };

        public OrderService(
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            IPaymentRepository paymentRepository,
            IOrderStatusHistoryRepository orderStatusHistoryRepository,
            IOrganizationMemberRepository orgMemberRepository,
            INotificationService notificationService)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _paymentRepository = paymentRepository;
            _orderStatusHistoryRepository = orderStatusHistoryRepository;
            _orgMemberRepository = orgMemberRepository;
            _notificationService = notificationService;
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

            // Determine the seller OrganizationId from the first product
            var sellerOrgId = products.First().OrganizationId;

            // All products must belong to the same organization
            if (products.Any(p => p.OrganizationId != sellerOrgId))
                return (null, "All products in a single order must belong to the same organization.");

            var orderItems = new List<OrderItem>();
            decimal totalAmount = 0;

            foreach (var product in products)
            {
                var requestedItem = orderDto.Items.First(i => i.ProductId == product.Id);
                var requestedQuantity = requestedItem.Quantity;

                if (product.Status != ProductStatus.Approved || !product.IsAvailableForDirectSale)
                    return (null, $"المنتج '{product.Name}' غير متاح للبيع المباشر حالياً");

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
                        ProductId = product.Id,
                        Quantity = requestedQuantity,
                        UnitPrice = itemPrice,
                    }
                );
            }

            var newOrder = new Order
            {
                BuyerId = orderDto.BuyerId,
                OrganizationId = sellerOrgId,
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

            // Create initial Payment record (COD - Pending)
            var payment = new Payment
            {
                OrderId = newOrder.Id,
                UserId = orderDto.BuyerId,
                Amount = totalAmount,
                PaymentMethod = PaymentMethod.CashOnDelivery,
                PaymentStatus = PaymentStatus.Pending
            };

            await _paymentRepository.AddAsync(payment);

            // Create initial status history entry
            var historyEntry = new OrderStatusHistory
            {
                OrderId = newOrder.Id,
                OldStatus = OrderStatus.Pending,
                NewStatus = OrderStatus.Pending,
                ChangedById = orderDto.BuyerId,
                ChangedAt = DateTime.UtcNow,
                Note = "Order created."
            };

            await _orderStatusHistoryRepository.AddAsync(historyEntry);
            await _orderRepository.SaveChangesAsync();

            return (newOrder, "تم إنشاء الطلب بنجاح");
        }

        public async Task<Order?> GetOrderByIdAsync(Guid orderId)
        {
            return await _orderRepository
                .GetTableNoTracking()
                .Include(o => o.OrderItems!)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Organization)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public IQueryable<Order> GetOrdersByBuyerIdQueryable(Guid buyerId)
        {
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
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.OrganizationId == organizationId)
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
            if (shippingFees < 0)
            {
                throw new ArgumentException("رسوم الشحن لا يمكن أن تكون أقل من الصفر");
            }

            var order = await _orderRepository
                .GetTableAsTracking()
                .Include(o => o.OrderItems!)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return false;

            if (newStatus == OrderStatus.Cancelled && order.Status != OrderStatus.Cancelled)
            {
                foreach (var item in order.OrderItems!)
                {
                    if (item.Product != null)
                    {
                        item.Product.StockQuantity += item.Quantity;
                    }
                }
            }

            order.Status = newStatus;
            order.ShippingFees = shippingFees;

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
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return (false, "الطلب غير موجود");

            if (order.BuyerId != buyerId)
                return (false, "غير مصرح لك بإلغاء هذا الطلب");

            if (order.Status != OrderStatus.Pending)
                return (false, "لا يمكن إلغاء الطلب في هذه المرحلة، تواصل مع البائع");

            foreach (var item in order.OrderItems!)
            {
                if (item.Product != null)
                {
                    item.Product.StockQuantity += item.Quantity;
                }
            }

            order.Status = OrderStatus.Cancelled;

            await _orderRepository.UpdateAsync(order);

            // Notify seller org members that the buyer cancelled
            try
            {
                var sellerMembers = await _orgMemberRepository.GetTableNoTracking()
                    .Where(m => m.OrganizationId == order.OrganizationId && m.IsActive)
                    .ToListAsync();

                foreach (var member in sellerMembers)
                {
                    await _notificationService.SendPushNotificationAsync(
                        member.UserId,
                        "Order Cancelled by Buyer",
                        $"The buyer has cancelled Order #{order.Id.ToString()[..8].ToUpper()}.",
                        order.Id.ToString());
                }
            }
            catch { }

            return (true, "تم إلغاء الطلب بنجاح واسترجاع الكميات للمخزون");
        }

        public async Task<string> UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus, Guid currentUserId, string? note = null)
        {
            var order = await _orderRepository.GetTableAsTracking()
                .Include(o => o.OrderItems!)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                throw new KeyNotFoundException("Order not found.");

            // 1. Validate state transition
            if (!_validTransitions.ContainsKey(order.Status) || !_validTransitions[order.Status].Contains(newStatus))
                throw new InvalidOperationException($"Invalid state transition: Cannot move from '{order.Status}' to '{newStatus}'.");

            var oldStatus = order.Status;

            // 2. Handle cancellation stock restore
            if (newStatus == OrderStatus.Cancelled)
            {
                foreach (var item in order.OrderItems!)
                {
                    if (item.Product != null)
                    {
                        item.Product.StockQuantity += item.Quantity;
                    }
                }
            }

            // 3. Update Order Status
            order.Status = newStatus;
            await _orderRepository.UpdateAsync(order);

            // 4. Add audit trail entry
            var historyEntry = new OrderStatusHistory
            {
                OrderId = orderId,
                OldStatus = oldStatus,
                NewStatus = newStatus,
                ChangedById = currentUserId,
                ChangedAt = DateTime.UtcNow,
                Note = note
            };

            await _orderStatusHistoryRepository.AddAsync(historyEntry);

            // 5. COD Business Rule: If Delivered, mark Payment as Paid
            if (newStatus == OrderStatus.Delivered)
            {
                var payment = await _paymentRepository.GetTableAsTracking()
                    .FirstOrDefaultAsync(p => p.OrderId == orderId);

                if (payment != null)
                {
                    payment.PaymentStatus = PaymentStatus.Paid;
                    payment.PaidAt = DateTime.UtcNow;
                    await _paymentRepository.UpdateAsync(payment);
                }
                else
                {
                    var newPayment = new Payment
                    {
                        OrderId = orderId,
                        UserId = order.BuyerId,
                        Amount = order.FinalTotal,
                        PaymentMethod = order.PaymentMethod,
                        PaymentStatus = PaymentStatus.Paid,
                        PaidAt = DateTime.UtcNow
                    };
                    await _paymentRepository.AddAsync(newPayment);
                }
            }

            await _orderRepository.SaveChangesAsync();

            // 6. Fire-and-forget notifications (wrapped to not break main flow)
            try
            {
                await FireNotificationsForStatusChangeAsync(order, newStatus);
            }
            catch { }

            return $"Order status updated from '{oldStatus}' to '{newStatus}' successfully.";
        }

        // =========================================================
        // Private: Notification Triggers per State Transition
        // =========================================================
        private async Task FireNotificationsForStatusChangeAsync(Order order, OrderStatus newStatus)
        {
            switch (newStatus)
            {
                case OrderStatus.Confirmed:
                    await _notificationService.SendPushNotificationAsync(
                        order.BuyerId,
                        "Order Confirmed ✅",
                        "The seller has confirmed your order and is preparing it.",
                        order.Id.ToString());
                    break;

                case OrderStatus.Delivered:
                    // Notify buyer
                    await _notificationService.SendPushNotificationAsync(
                        order.BuyerId,
                        "Order Delivered 📦",
                        "Your order has been delivered. Please leave a review!",
                        order.Id.ToString());

                    // Notify all active seller org members
                    var sellerMembers = await _orgMemberRepository.GetTableNoTracking()
                        .Where(m => m.OrganizationId == order.OrganizationId && m.IsActive)
                        .ToListAsync();

                    foreach (var member in sellerMembers)
                    {
                        await _notificationService.SendPushNotificationAsync(
                            member.UserId,
                            "Order Delivered & Paid 💰",
                            "The buyer has received the order and COD has been settled.",
                            order.Id.ToString());
                    }
                    break;

                case OrderStatus.Cancelled:
                    // If the person who changed the status is the buyer, notify sellers
                    // If the person who changed is a seller member, notify buyer
                    var isBuyerCancelling = order.BuyerId == order.BuyerId; // always true – seller cancel is via UpdateOrderStatus
                    // We notify the buyer when a seller cancels
                    await _notificationService.SendPushNotificationAsync(
                        order.BuyerId,
                        "Order Cancelled ❌",
                        "Your order has been cancelled by the seller. Please contact support if needed.",
                        order.Id.ToString());

                    // Also notify seller org in case it was the buyer who cancelled via state machine
                    var orgMembers = await _orgMemberRepository.GetTableNoTracking()
                        .Where(m => m.OrganizationId == order.OrganizationId && m.IsActive)
                        .ToListAsync();

                    foreach (var member in orgMembers)
                    {
                        await _notificationService.SendPushNotificationAsync(
                            member.UserId,
                            "Order Cancelled ❌",
                            $"Order #{order.Id.ToString()[..8].ToUpper()} has been cancelled.",
                            order.Id.ToString());
                    }
                    break;
            }
        }
    }
}
