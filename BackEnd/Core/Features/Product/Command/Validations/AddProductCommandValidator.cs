using Core.Features.Product.Command.Models;

namespace Core.Features.Product.Command.Validations
{
    public class AddProductCommandValidator : AbstractValidator<AddProductCommand>
    {
        public AddProductCommandValidator()
        {
            RuleFor(x => x.OrganizationId)
                .NotEmpty().WithMessage("يجب تحديد المؤسسة التابع لها المنتج");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("اسم المنتج مطلوب")
                .MaximumLength(150).WithMessage("اسم المنتج يجب ألا يتجاوز 150 حرف");

            RuleFor(x => x.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("الكمية المتاحة لا يمكن أن تكون بالسالب");

            RuleFor(x => x.ProductType)
                .IsInEnum().WithMessage("نوع المنتج غير صحيح");

            RuleFor(x => x)
                .Must(x => x.IsAvailableForDirectSale || x.IsAvailableForAuction)
                .WithMessage("يجب اختيار طريقة بيع واحدة على الأقل (بيع مباشر أو مزاد)");

            When(x => x.IsAvailableForDirectSale, () =>
            {
                RuleFor(x => x.DirectSalePrice)
                    .GreaterThan(0).WithMessage("سعر البيع المباشر يجب أن يكون أكبر من الصفر");
            });

            When(x => x.IsAvailableForAuction, () =>
            {
                RuleFor(x => x.StartBiddingPrice)
                    .GreaterThan(0).WithMessage("سعر فتح المزاد يجب أن يكون أكبر من الصفر");
            });
        }
    }
}