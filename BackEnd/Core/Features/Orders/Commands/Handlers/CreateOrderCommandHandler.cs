using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Base;
using Core.Features.Orders.Commands.Models;
using MediatR;
using Service.Services.OrderService;

namespace Core.Features.Orders.Commands.Handlers
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Response<string>>
    {
        private readonly IOrderService _orderService;

        public CreateOrderCommandHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<Response<string>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            // 1. تحويل الـ List اللي جاية من الفرونت إند لـ Dictionary عشان السيرفيس تفهمه
            // بحيث الـ Key هو ProductId والـ Value هي Quantity
            var productQuantities = request.Items.ToDictionary(
                item => item.ProductId,
                item => item.Quantity
            );

            // 2. إرسال البيانات للسيرفيس عشان تحسب السعر وتخصم من المخزون وتكريت الطلب
            var (order, message) = await _orderService.CreateOrderAsync(request.BuyerId, productQuantities);

            // 3. التحقق من النتيجة
            if (order == null)
            {
                // معناها إن في منتج خلصان من المخزون أو سعره فيه مشكلة أو مش متاح للبيع
                return new Response<string>(message) { Succeeded = false };
            }

            // 4. إرجاع رسالة النجاح (وممكن نرجع الـ order.Id لو الفرونت إند بيحتاجه عشان يفتح صفحة الفاتورة)
            return new Response<string>(message) { Succeeded = true };
        }
    }
}   