using Core.Features.Orders.Commands.Models;
using FluentValidation;

namespace Core.Features.Orders.Commands.Validations
{
    public class ChangeOrderStatusCommandValidator : AbstractValidator<ChangeOrderStatusCommand>
    {
        public ChangeOrderStatusCommandValidator()
        {
            // ShippingFees validation removed as the command no longer handles fees
        }
    }
}
