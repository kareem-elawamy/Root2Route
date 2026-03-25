using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Base;
using Core.Features.Reviews.Queries.Models;
using Core.Features.Reviews.Queries.Results;
using MediatR;
using Service.Services.ReviewService;

namespace Core.Features.Reviews.Queries.Handlers
{
    public class GetOrganizationReviewsQueryHandler : IRequestHandler<GetOrganizationReviewsQuery, Response<List<ReviewResponse>>>
    {
        private readonly IReviewService _reviewService;

        public GetOrganizationReviewsQueryHandler(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        public async Task<Response<List<ReviewResponse>>> Handle(GetOrganizationReviewsQuery request, CancellationToken cancellationToken)
        {
            request.PageSize = request.PageSize > 50 ? 50 : request.PageSize;

            var (reviews, totalCount) = await _reviewService.GetOrganizationReviewsAsync(
                request.OrganizationId,
                request.PageNumber,
                request.PageSize);

            var mapped = reviews.Select(r => new ReviewResponse
            {
                Id = r.Id,
                ReviewerId = r.ReviewerId,
                ReviewerName = r.Reviewer?.FullName,
                ProductId = r.ProductId,
                ProductName = r.Product?.Name,
                OrderId = r.OrderId,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            }).ToList();

            return new Response<List<ReviewResponse>>(mapped) 
            { 
                Succeeded = true,
                Meta = new { TotalCount = totalCount, request.PageNumber, request.PageSize }
            };
        }
    }
}
