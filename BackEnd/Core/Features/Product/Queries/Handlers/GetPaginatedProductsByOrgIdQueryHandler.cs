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
    public class GetPaginatedProductsByOrgIdQueryHandler : IRequestHandler<GetPaginatedProductsByOrgIdQuery, PaginatedResult<ProductResponse>>
    {
        private readonly IProductService _productService;

        public GetPaginatedProductsByOrgIdQueryHandler(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<PaginatedResult<ProductResponse>> Handle(GetPaginatedProductsByOrgIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _productService.GetPaginatedProductsByOrgIdAsync(request.OrganizationId, request.PageNumber, request.PageSize);

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

                Images = p.Images != null ? p.Images.Select(i => i.ImageUrl).ToList() : new List<string>(),
                MainImageUrl = p.Images?.FirstOrDefault(i => i.IsMain)?.ImageUrl ?? p.Images?.FirstOrDefault()?.ImageUrl

            }).ToList();

            return new PaginatedResult<ProductResponse>(mappedProducts, result.TotalCount, request.PageNumber, request.PageSize);
        }
    }
}