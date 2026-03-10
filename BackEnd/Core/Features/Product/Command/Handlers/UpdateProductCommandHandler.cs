using System.Threading;
using System.Threading.Tasks;
using Core.Base;
using Core.Features.Product.Command.Models;
using MediatR;
using Service.Services.ProductService;

namespace Core.Features.Product.Command.Handlers
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Response<string>>
    {
        private readonly IProductService _productService;
        public UpdateProductCommandHandler(IProductService productService) { _productService = productService; }

        public async Task<Response<string>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _productService.GetProductByIdAsync(request.Id);
            if (product == null) return new Response<string>("المنتج غير موجود") { Succeeded = false };

            // فحص الباركود لو اتغير
            if (!string.IsNullOrWhiteSpace(request.Barcode) && request.Barcode != product.Barcode)
            {
                if (await _productService.IsBarcodeExistAsync(request.Barcode))
                    return new Response<string>("هذا الباركود مسجل لمنتج آخر مسبقاً");
            }

            // تعديل البيانات
            product.Name = request.Name;
            product.Description = request.Description;
            product.StockQuantity = request.StockQuantity;
            product.IsAvailableForDirectSale = request.IsAvailableForDirectSale;
            product.DirectSalePrice = request.IsAvailableForDirectSale ? request.DirectSalePrice : 0;
            product.IsAvailableForAuction = request.IsAvailableForAuction;
            product.StartBiddingPrice = request.IsAvailableForAuction ? request.StartBiddingPrice : 0;
            product.Barcode = request.Barcode;
            product.ExpiryDate = request.ExpiryDate;
            product.WeightUnit = request.WeightUnit;
            product.ProductType = request.ProductType;

            await _productService.UpdateProductAsync(product);
            return new Response<string>("تم تعديل المنتج بنجاح") { Succeeded = true };
        }
    }
}