using FluentValidation;
using Core.Features.Auctions.Commands.Models;
using System;

namespace Core.Features.Auctions.Commands.Validators
{
    public class PlaceBidCommandValidator : AbstractValidator<PlaceBidCommand>
    {
        public PlaceBidCommandValidator()
        {
            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Bid amount must be greater than zero.");
                
            RuleFor(x => x.AuctionId)
                .NotEmpty().WithMessage("Auction ID is required.");
        }
    }
}
