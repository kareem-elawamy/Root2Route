using Core.Features.Product.Command.Models;
using Service.Services.ProductService;

namespace Core.Features.Product.Command.Handlers
{
    public class AddProductCommandHandler : IRequestHandler<AddProductCommand, Response<string>>
    {
        private readonly IProductService _productService;

        public AddProductCommandHandler(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<Response<string>> Handle(AddProductCommand request, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(request.Barcode))
            {
                var isExist = await _productService.IsBarcodeExistAsync(request.Barcode);
                if (isExist) return new Response<string>("هذا الباركود مسجل لمنتج آخر مسبقاً");
            }

            // Mapping
            var newProduct = new Domain.Models.Product
            {
                OrganizationId = request.OrganizationId,

                Name = request.Name,
                Description = request.Description,
                ImageUrl = request.ImageUrl, // (لو بتعمل Upload لصورة، هتهندل رفع الصورة هنا الأول وتحط اللينك)
                StockQuantity = request.StockQuantity,

                IsAvailableForDirectSale = request.IsAvailableForDirectSale,
                DirectSalePrice = request.IsAvailableForDirectSale ? request.DirectSalePrice : 0,

                IsAvailableForAuction = request.IsAvailableForAuction,
                StartBiddingPrice = request.IsAvailableForAuction ? request.StartBiddingPrice : 0,

                Barcode = request.Barcode,
                ExpiryDate = request.ExpiryDate,
                WeightUnit = request.WeightUnit,
                ProductType = request.ProductType
            };

            await _productService.AddProductAsync(newProduct);

            return new Response<string>("تمت إضافة المنتج بنجاح") { Succeeded = true };
        }
    }
}