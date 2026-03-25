using FluentValidation;
using Core.Features.Chat.Commands.Models;
using Domain.Enums;

namespace Core.Features.Chat.Commands.Validators
{
    public class SendMessageCommandValidator : AbstractValidator<SendMessageCommand>
    {
        public SendMessageCommandValidator()
        {
            RuleFor(x => x.ChatRoomId).NotEmpty().WithMessage("ChatRoomId is required.");
            
            RuleFor(x => x.Type)
                .Must(t => t != MessageType.OfferAccepted && t != MessageType.OfferRejected)
                .WithMessage("System-level message types (Accept/Reject) cannot be manually transmitted via API.");
            
            RuleFor(x => x.Content)
                .Must(c => !string.IsNullOrWhiteSpace(c)).WithMessage("Message content cannot be inherently empty or localized whitespace.")
                .When(x => x.Type == MessageType.Text);

            When(x => x.Type == MessageType.NegotiationOffer, () => {
                RuleFor(x => x.ProposedPrice)
                    .NotNull().WithMessage("Proposed price is required for negotiation offers.")
                    .GreaterThan(0).WithMessage("Proposed price must be strictly greater than zero.");
                    
                RuleFor(x => x.ProposedQuantity)
                    .NotNull().WithMessage("Proposed quantity is required for negotiation offers.")
                    .GreaterThan(0).WithMessage("Proposed quantity must be strictly greater than zero.");
            });
        }
    }
}
