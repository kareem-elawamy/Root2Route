using Core.Base;
using Core.Features.Product.Queries.Results;
using Domain.Enums;
using MediatR;

namespace Core.Features.Product.Queries.Models
{
    // لاحظ إننا بقينا نرجع PaginatedResult بدل Response العادي
    public class GetPaginatedProductsQuery : IRequest<PaginatedResult<ProductResponse>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public ProductStatus? Status { get; set; } = ProductStatus.Approved; // الديفولت Approved للناس

        // للبحث والفلترة (اختياري)
        public string? Search { get; set; }
        public ProductType? ProductType { get; set; }
    }
}