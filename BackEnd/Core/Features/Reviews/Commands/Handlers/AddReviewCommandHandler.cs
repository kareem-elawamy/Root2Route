using System.Threading;
using System.Threading.Tasks;
using Core.Base;
using Core.Features.Reviews.Commands.Models;
using MediatR;
using Service.Services.ReviewService;

namespace Core.Features.Reviews.Commands.Handlers
{
    public class AddReviewCommandHandler : IRequestHandler<AddReviewCommand, Response<Guid>>
    {
        private readonly IReviewService _reviewService;

        public AddReviewCommandHandler(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        public async Task<Response<Guid>> Handle(AddReviewCommand request, CancellationToken cancellationToken)
        {
            var review = await _reviewService.AddReviewAsync(
                request.CurrentUserId,
                request.TargetOrganizationId,
                request.OrderId,
                request.ProductId,
                request.Rating,
                request.Comment);

            return new Response<Guid>(review.Id) { Succeeded = true };
        }
    }
}
