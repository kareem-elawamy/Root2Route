using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Features.Product.Queries.Results;

namespace Core.Features.Product.Queries.Models
{
    public class GetProductOverviewModel : IRequest<Response<ProductOverviewResponse>>
    {
        public Guid OrganizationId { get; set; }
    }
}