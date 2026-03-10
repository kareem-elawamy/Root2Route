using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enums;
using Domain.Models;
using Infrastructure.Repositories.OrderRepository;
using Infrastructure.Repositories.ProductRepository;
using Microsoft.EntityFrameworkCore;

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

        public async Task<(Order? Order, string Message)> CreateOrderAsync(Guid buyerId, Dictionary<Guid, int> productQuantities)
        {
            var productIds = productQuantities.Keys.ToList();

            var products = await _productRepository.GetTableAsTracking()
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync();

            if (products.Count != productIds.Count)
                return (null, "عفواً، بعض المنتجات المطلوبة غير موجودة في النظام");

            var orderItems = new List<OrderItem>();
            decimal totalAmount = 0;

            foreach (var product in products)
            {
                var requestedQuantity = productQuantities[product.Id];

                // التأكد إن المنتج تمت الموافقة عليه ومتاح للبيع المباشر أصلاً
                if (product.Status != ProductStatus.Approved || !product.IsAvailableForDirectSale)
                    return (null, $"المنتج '{product.Name}' غير متاح للبيع المباشر حالياً");

                // التأكد إن المخزون يكفي الكمية المطلوبة
                if (product.StockQuantity < requestedQuantity)
                    return (null, $"الكمية المطلوبة من '{product.Name}' تتجاوز المخزون المتاح ({product.StockQuantity} فقط)");

                // حساب السعر وتجهيز عنصر الطلب (بناخد السعر من الداتابيز مش من اليوزر للحماية)
                var itemPrice = product.DirectSalePrice;
                totalAmount += itemPrice * requestedQuantity;

                orderItems.Add(new OrderItem
                {
                    productid = product.Id,
                    Quantity = requestedQuantity,
                    UnitPrice = itemPrice
                });

                // 4. خصم الكمية المباعة من المخزون الفعلي للمنتج
                product.StockQuantity -= requestedQuantity;
            }

            // 5. إنشاء كائن الطلب (Order)
            var newOrder = new Order
            {
                BuyerId = buyerId,
                OrderDate = DateTime.UtcNow,
                TotalAmount = totalAmount,
                Status = OrderStatus.Pending,
                OrderItems = orderItems
            };

            // 6. الحفظ في الداتابيز
            // EF Core هيحفظ الـ Order والـ OrderItems ويعمل Update للـ Stock بتاع الـ Products في Transaction واحد أوتوماتيك!
            await _orderRepository.AddAsync(newOrder);

            return (newOrder, "تم إنشاء الطلب بنجاح");
        }

        public async Task<Order?> GetOrderByIdAsync(Guid orderId)
        {
            return await _orderRepository.GetTableNoTracking()
                .Include(o => o.OrderItems!)
                    .ThenInclude(oi => oi.product) // عشان نجيب بيانات المنتج (زي اسمه) جوا الـ OrderItem
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<List<Order>> GetOrdersByBuyerIdAsync(Guid buyerId)
        {
            return await _orderRepository.GetTableNoTracking()
                .Include(o => o.OrderItems!)
                    .ThenInclude(oi => oi.product)
                .Where(o => o.BuyerId == buyerId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }
        public async Task<(List<Order> Orders, int TotalCount)> GetPaginatedReceivedOrdersForOrgAsync(Guid organizationId, int pageNumber, int pageSize, OrderStatus? status = null)
        {
            // 1. نبني الـ Query الأساسي ونجيب الطلبات اللي تخص المنظمة دي
            var query = _orderRepository.GetTableNoTracking()
                .Include(o => o.OrderItems!)
                    .ThenInclude(oi => oi.product)
                .Where(o => o.OrderItems!.Any(oi => oi.product!.OrganizationId == organizationId))
                .AsQueryable();

            // 2. تطبيق الفلتر (لو البائع عايز الطلبات الـ Pending بس مثلاً)
            if (status.HasValue)
            {
                query = query.Where(o => o.Status == status.Value);
            }

            // 3. حساب العدد الإجمالي للطلبات (مهم للباجنيشن)
            var totalCount = await query.CountAsync();

            // 4. تطبيق الباجنيشن والترتيب (الأحدث أولاً)
            var orders = await query
                .OrderByDescending(o => o.OrderDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (orders, totalCount);
        }
        public async Task<bool> ChangeOrderStatusAsync(Guid orderId, OrderStatus newStatus)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null) return false;

            order.Status = newStatus;
            await _orderRepository.UpdateAsync(order);

            return true;
        }
    }
}