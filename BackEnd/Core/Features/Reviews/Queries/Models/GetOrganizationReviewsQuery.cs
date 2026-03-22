using System;
using System.Collections.Generic;
using Core.Base;
using Core.Features.Reviews.Queries.Results;
using MediatR;

namespace Core.Features.Reviews.Queries.Models
{
    public class GetOrganizationReviewsQuery : IRequest<Response<List<ReviewResponse>>>
    {
        public Guid OrganizationId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
