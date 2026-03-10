using System.Linq; // مهم عشان نقدر نستخدم Select و FirstOrDefault
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Core.Base;
using Core.Features.Product.Queries.Models;
using Core.Features.Product.Queries.Results;
using MediatR;
using Service.Services.ProductService;

namespace Core.Features.Product.Queries.Handlers
{
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Response<ProductResponse>>
    {
        private readonly IProductService _productService;

        public GetProductByIdQueryHandler(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<Response<ProductResponse>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            // 1. جلب المنتج (الآن سيأتي مع صوره بفضل الـ Include في السيرفيس)
            var product = await _productService.GetProductByIdAsync(request.Id);

            // 2. التحقق من وجوده
            if (product == null)
            {
                return new Response<ProductResponse>("المنتج غير موجود أو تم حذفه") { Succeeded = false };
            }

            // 3. التحويل (Mapping)
            var mappedProduct = new ProductResponse
            {
                Id = product.Id,
                OrganizationId = product.OrganizationId,
                Name = product.Name,
                Description = product.Description,
                StockQuantity = product.StockQuantity,
                IsAvailableForDirectSale = product.IsAvailableForDirectSale,
                DirectSalePrice = product.DirectSalePrice,
                IsAvailableForAuction = product.IsAvailableForAuction,
                StartBiddingPrice = product.StartBiddingPrice,
                Barcode = product.Barcode,
                ExpiryDate = product.ExpiryDate,
                WeightUnit = product.WeightUnit?.ToString(), 
                ProductType = product.ProductType.ToString(),

                Images = product.Images != null
                            ? product.Images.Select(i => i.ImageUrl).ToList()
                            : new List<string>(),

                MainImageUrl = product.Images?.FirstOrDefault(i => i.IsMain)?.ImageUrl
                            ?? product.Images?.FirstOrDefault()?.ImageUrl
            };

            return new Response<ProductResponse>(mappedProduct) { Succeeded = true };
        }
    }
}