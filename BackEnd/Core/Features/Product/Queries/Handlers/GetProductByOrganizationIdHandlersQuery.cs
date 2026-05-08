using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Features.Product.Queries.Models;
using Core.Features.Product.Queries.Results;
using Service;
using Service.Services.ProductService;

namespace Core.Features.Product.Queries.Handlers
{
    public class GetProductByOrganizationIdHandlersQuery : ResponseHandler
    , IRequestHandler<GetProductByOrganizationId, Response<List<ProductResponse>>>,
        IRequestHandler<GetProductOverviewModel, Response<ProductOverviewResponse>>
    {

        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public GetProductByOrganizationIdHandlersQuery(IProductService productService, IMapper mapper)
        {
            _productService = productService;
            _mapper = mapper;
        }

        public Task<Response<List<ProductResponse>>> Handle(GetProductByOrganizationId request, CancellationToken cancellationToken)
        {
            // 1. جلب المنتجات بناءً على OrganizationId
            var products = _productService.GetProductsByOrganizationIdAsync(request.OrganizationId, request.PageNumber, request.PageSize).Result;


            if (products == null || !products.Any())
            {
                return Task.FromResult(NotFound<List<ProductResponse>>("لا توجد منتجات لهذه المنظمة"));
            }

            var mappedProducts = products.Select(p => new ProductResponse
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

            return Task.FromResult(new Response<List<ProductResponse>>(mappedProducts) { Succeeded = true });
        }

        public Task<Response<ProductOverviewResponse>> Handle(GetProductOverviewModel request, CancellationToken cancellationToken)
        {
            var overview = _productService.GetProductOverviewByOrganizationIdAsync(request.OrganizationId).Result;
            var overviewResponse = new ProductOverviewResponse
            {
                TotalProducts = overview.TotalProducts,
                ActiveProducts = overview.ActiveProducts,
                LowStockProducts = overview.LowStockProducts,
                InventoryValue = overview.InventoryValue
            };
            return Task.FromResult(new Response<ProductOverviewResponse>(overviewResponse) { Succeeded = true });
        }
    }
}
