using Core.Features.Orders.Commands.Models;
using FluentValidation;

namespace Core.Features.Orders.Commands.Validations
{
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(x => x.BuyerId)
                .NotEmpty().WithMessage("يجب تحديد المشتري");

            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("يجب إضافة منتج واحد على الأقل لإنشاء الطلب");

            // دي Rule بتلف على كل عنصر جوه لستة المشتريات تتأكد منه
            RuleForEach(x => x.Items).ChildRules(items =>
            {
                items.RuleFor(i => i.ProductId)
                    .NotEmpty().WithMessage("يجب تحديد المنتج بشكل صحيح");

                items.RuleFor(i => i.Quantity)
                    .GreaterThan(0).WithMessage("الكمية المطلوبة يجب أن تكون أكبر من الصفر");
            });
        }
    }
}