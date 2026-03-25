using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enums;
using Domain.Models;
using Infrastructure.Repositories.OrderRepository;
using Infrastructure.Repositories.OrganizationMemberRepository;
using Infrastructure.Repositories.ReviewRepository;
using Microsoft.EntityFrameworkCore;

namespace Service.Services.ReviewService
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrganizationMemberRepository _orgMemberRepo;

        public ReviewService(
            IReviewRepository reviewRepository, 
            IOrderRepository orderRepository,
            IOrganizationMemberRepository orgMemberRepo)
        {
            _reviewRepository = reviewRepository;
            _orderRepository = orderRepository;
            _orgMemberRepo = orgMemberRepo;
        }

        public async Task<Review> AddReviewAsync(Guid reviewerId, Guid targetOrganizationId, Guid orderId, Guid? productId, int rating, string? comment)
        {
            // Rule 1: The order must exist and belong to this buyer
            var order = await _orderRepository.GetTableNoTracking()
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                throw new KeyNotFoundException("Order not found.");

            if (order.BuyerId != reviewerId)
                throw new UnauthorizedAccessException("You can only review orders you have placed.");

            // Rule 2: Order must be Delivered
            if (order.Status != OrderStatus.Delivered)
                throw new InvalidOperationException("You can only review a Delivered order.");

            // Rule 3: The seller org must match
            if (order.OrganizationId != targetOrganizationId)
                throw new InvalidOperationException("The target organization does not match the order's seller.");

            // Rule 3.5: Prevent shill reviews (seller reviewing own org)
            bool isSelfReview = await _orgMemberRepo.GetTableNoTracking()
                .AnyAsync(m => m.OrganizationId == targetOrganizationId && m.UserId == reviewerId);

            if (isSelfReview)
                throw new InvalidOperationException("Sellers cannot review their own organization.");

            // Rule 4: Prevent duplicate reviews for the same OrderId
            var alreadyReviewed = await _reviewRepository.GetTableNoTracking()
                .AnyAsync(r => r.OrderId == orderId && r.ReviewerId == reviewerId);

            if (alreadyReviewed)
                throw new InvalidOperationException("You have already reviewed this order.");

            var review = new Review
            {
                ReviewerId = reviewerId,
                TargetOrganizationId = targetOrganizationId,
                OrderId = orderId,
                ProductId = productId,
                Rating = rating,
                Comment = comment
            };

            await _reviewRepository.AddAsync(review);
            await _reviewRepository.SaveChangesAsync();

            return review;
        }

        public async Task<(List<Review> Reviews, int TotalCount)> GetOrganizationReviewsAsync(Guid organizationId, int pageNumber, int pageSize)
        {
            var query = _reviewRepository.GetTableNoTracking()
                .Where(r => r.TargetOrganizationId == organizationId)
                .Include(r => r.Reviewer)
                .Include(r => r.Product);

            var totalCount = await query.CountAsync();

            var reviews = await query
                .OrderByDescending(r => r.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (reviews, totalCount);
        }
    }
}
