using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Base;
using Core.Features.Reviews.Commands.Models;
using MediatR;
using Service.Services.ReviewService;

namespace Core.Features.Reviews.Commands.Handlers
{
    public class AddReviewCommandHandler : ResponseHandler, IRequestHandler<AddReviewCommand, Response<Guid>>
    {
        private readonly IReviewService _reviewService;

        public AddReviewCommandHandler(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        public async Task<Response<Guid>> Handle(AddReviewCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var review = await _reviewService.AddReviewAsync(
                    request.CurrentUserId,
                    request.TargetOrganizationId,
                    request.OrderId,
                    request.ProductId,
                    request.Rating,
                    request.Comment);

                return Success(review.Id);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound<Guid>(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized<Guid>(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest<Guid>(ex.Message);
            }
            catch (Exception ex)
            {
                // Catch-all: prevents unhandled exceptions from bubbling up as unformatted 500 errors.
                // Log the full exception details here if a logger is available.
                return BadRequest<Guid>($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
