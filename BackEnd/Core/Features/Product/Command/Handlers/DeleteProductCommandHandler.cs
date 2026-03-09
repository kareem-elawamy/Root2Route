using System.Threading;
using System.Threading.Tasks;
using Core.Base;
using Core.Features.Product.Command.Models;
using MediatR;
using Service.Services.ProductService;

namespace Core.Features.Product.Command.Handlers
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Response<string>>
    {
        private readonly IProductService _productService;
        public DeleteProductCommandHandler(IProductService productService) { _productService = productService; }

        public async Task<Response<string>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _productService.GetProductByIdAsync(request.Id);
            if (product == null) return new Response<string>("المنتج غير موجود") { Succeeded = false };

            await _productService.DeleteProductAsync(product);
            return new Response<string>("تم حذف المنتج بنجاح") { Succeeded = true };
        }
    }
}