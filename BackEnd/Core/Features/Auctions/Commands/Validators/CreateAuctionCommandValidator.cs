using FluentValidation;
using Core.Features.Auctions.Commands.Models;
using System;

namespace Core.Features.Auctions.Commands.Validators
{
    public class CreateAuctionCommandValidator : AbstractValidator<CreateAuctionCommand>
    {
        public CreateAuctionCommandValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(150).WithMessage("Title cannot exceed 150 characters.");

            RuleFor(x => x.StartDate)
                .GreaterThanOrEqualTo(DateTime.UtcNow.AddMinutes(-5)).WithMessage("Start date must be in the future.");

            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate).WithMessage("End date must be after the start date.");

            RuleFor(x => x.StartPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Start price cannot be negative.");

            RuleFor(x => x.MinimumBidIncrement)
                .GreaterThan(0).WithMessage("Minimum bid increment must be greater than zero.");

            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("Product ID is required.");
        }
    }
}
