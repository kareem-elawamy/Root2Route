using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Models;

namespace Service.Services.ReviewService
{
    public interface IReviewService
    {
        Task<Review> AddReviewAsync(Guid reviewerId, Guid targetOrganizationId, Guid orderId, Guid? productId, int rating, string? comment);
        Task<(List<Review> Reviews, int TotalCount)> GetOrganizationReviewsAsync(Guid organizationId, int pageNumber, int pageSize);
    }
}
