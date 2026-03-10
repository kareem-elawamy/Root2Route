using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Base;
using Core.Features.Orders.Commands.Models;
using MediatR;
using Service.Services.OrderService;

namespace Core.Features.Orders.Commands.Handlers
{
    public class ChangeOrderStatusCommandHandler : IRequestHandler<ChangeOrderStatusCommand, Response<string>>
    {
        private readonly IOrderService _orderService;

        public ChangeOrderStatusCommandHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<Response<string>> Handle(ChangeOrderStatusCommand request, CancellationToken cancellationToken)
        {
            // 1. نجيب الطلب بكل تفاصيله
            var order = await _orderService.GetOrderByIdAsync(request.OrderId);
            if (order == null)
                return new Response<string>("الطلب غير موجود") { Succeeded = false };

            // 2. 👈 حماية البيزنس: نتأكد إن البائع ده له منتجات جوه الطلب ده
            var isSellerOwnsItems = order.OrderItems!.Any(oi => oi.product?.OrganizationId == request.OrganizationId);

            if (!isSellerOwnsItems)
                return new Response<string>("غير مصرح لك بتعديل حالة هذا الطلب لأنه لا يحتوي على منتجاتك") { Succeeded = false };

            // 3. تغيير الحالة
            await _orderService.ChangeOrderStatusAsync(request.OrderId, request.NewStatus);

            return new Response<string>($"تم تغيير حالة الطلب إلى {request.NewStatus} بنجاح") { Succeeded = true };
        }
    }
}