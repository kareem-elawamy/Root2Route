using Core.Features.Orders.Commands.Models;
using FluentValidation;

namespace Core.Features.Orders.Commands.Validations
{
    public class ChangeOrderStatusCommandValidator : AbstractValidator<ChangeOrderStatusCommand>
    {
        public ChangeOrderStatusCommandValidator()
        {
            RuleFor(x => x.ShippingFees)
                .GreaterThanOrEqualTo(0)
                .WithMessage("رسوم الشحن لا يمكن أن تكون قيمة سالبة.");
        }
    }
}
