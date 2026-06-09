using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Features.Product.Queries.Results;

namespace Core.Features.Product.Queries.Models
{
    public class GetProductByOrganizationId : IRequest<Response<List<ProductResponse>>>
    {
        public Guid OrganizationId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}