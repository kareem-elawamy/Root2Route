using FluentValidation;
using Core.Features.Chat.Queries.Models;

namespace Core.Features.Chat.Queries.Validators
{
    public class GetChatHistoryQueryValidator : AbstractValidator<GetChatHistoryQuery>
    {
        public GetChatHistoryQueryValidator()
        {
            RuleFor(x => x.ChatRoomId)
                .NotEmpty().WithMessage("ChatRoomId is structurally required to execute.");

            RuleFor(x => x.PageNumber)
                .GreaterThan(0).WithMessage("PageNumber must legally resolve as a positive integer greater than zero.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100).WithMessage("PageSize is strictly bounded between 1 and 100 ensuring structural API safety.");
        }
    }
}
