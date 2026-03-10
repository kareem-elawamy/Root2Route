using System.Threading;
using System.Threading.Tasks;
using Core.Base;
using Core.Features.Product.Command.Models;
using MediatR;
using Service.Services.ProductService;

namespace Core.Features.Product.Command.Handlers
{
    public class ChangeProductStatusCommandHandler : IRequestHandler<ChangeProductStatusCommand, Response<string>>
    {
        private readonly IProductService _productService;

        public ChangeProductStatusCommandHandler(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<Response<string>> Handle(ChangeProductStatusCommand request, CancellationToken cancellationToken)
        {
            var isChanged = await _productService.ChangeProductStatusAsync(request.ProductId, request.Status, request.RejectionReason);

            if (!isChanged)
                return new Response<string>("المنتج غير موجود") { Succeeded = false };

            return new Response<string>($"تم تغيير حالة المنتج إلى {request.Status} بنجاح") { Succeeded = true };
        }
    }
}