using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Base;
using Core.Features.Product.Queries.Models;
using Core.Features.Product.Queries.Results;
using MediatR;
using Service.Services.ProductService;

namespace Core.Features.Product.Queries.Handlers
{
    public class GetPaginatedProductsQueryHandler : IRequestHandler<GetPaginatedProductsQuery, PaginatedResult<ProductResponse>>
    {
        private readonly IProductService _productService;

        public GetPaginatedProductsQueryHandler(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<PaginatedResult<ProductResponse>> Handle(GetPaginatedProductsQuery request, CancellationToken cancellationToken)
        {
            // 1. جلب البيانات من السيرفيس (هترجعنا الـ Tuple اللي فيها المنتجات والعدد الكلي)
            var result = await _productService.GetPaginatedProductsAsync(
                request.PageNumber,
                request.PageSize,
                request.Search,
                request.ProductType,
                request.Status);

            // 2. تحويل الـ Entity لـ DTO (Mapping)
            var mappedProducts = result.Products.Select(p => new ProductResponse
            {
                Id = p.Id,
                OrganizationId = p.OrganizationId,
                Name = p.Name,
                Description = p.Description,
                StockQuantity = p.StockQuantity,
                IsAvailableForDirectSale = p.IsAvailableForDirectSale,
                DirectSalePrice = p.DirectSalePrice,
                IsAvailableForAuction = p.IsAvailableForAuction,
                StartBiddingPrice = p.StartBiddingPrice,
                Barcode = p.Barcode,
                ExpiryDate = p.ExpiryDate,
                WeightUnit = p.WeightUnit?.ToString(),
                ProductType = p.ProductType.ToString(),

                Images = p.Images != null
                            ? p.Images.Select(i => i.ImageUrl).ToList()
                            : new List<string>(),

                MainImageUrl = p.Images?.FirstOrDefault(i => i.IsMain)?.ImageUrl
                            ?? p.Images?.FirstOrDefault()?.ImageUrl

            }).ToList();

            // 3. إرجاع النتيجة جوه الـ PaginatedResult عشان تروح للفرونت إند مظبوطة
            return new PaginatedResult<ProductResponse>(
                mappedProducts,
                result.TotalCount,
                request.PageNumber,
                request.PageSize);
        }
    }
}