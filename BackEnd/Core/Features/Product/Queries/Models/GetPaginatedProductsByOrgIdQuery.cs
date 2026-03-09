using System;
using Core.Base;
using Core.Features.Product.Queries.Results;
using MediatR;

namespace Core.Features.Product.Queries.Models
{
    public class GetPaginatedProductsByOrgIdQuery : IRequest<PaginatedResult<ProductResponse>>
    {
        public Guid OrganizationId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}