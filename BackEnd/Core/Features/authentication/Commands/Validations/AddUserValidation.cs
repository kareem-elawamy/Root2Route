using Core.Features.authentication.Commands.Models;
using Core.Resources;
using Domain.Enums;
using FluentValidation;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Features.authentication.Commands.Validations
{
    public class AddUserValidation : AbstractValidator<AddUserCommand>
    {
        private readonly IStringLocalizer<SharedResources> _localizer;

        #region Handle Functions
        public void ApplyValidationsRules()
        {
            RuleFor(x => x.FullName)
                 .NotEmpty().WithMessage(_localizer[SharedResourcesKeys.NotEmpty])
                 .NotNull().WithMessage(_localizer[SharedResourcesKeys.Required])
                 .MaximumLength(100).WithMessage(_localizer[SharedResourcesKeys.MaxLengthis100]);

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage(_localizer[SharedResourcesKeys.NotEmpty])
                .NotNull().WithMessage(_localizer[SharedResourcesKeys.Required])
                .MaximumLength(100).WithMessage(_localizer[SharedResourcesKeys.MaxLengthis100]);

            RuleFor(x => x.Email)
                 .NotEmpty().WithMessage(_localizer[SharedResourcesKeys.NotEmpty])
                 .NotNull().WithMessage(_localizer[SharedResourcesKeys.Required]);
            RuleFor(x => x.Password)
                 .NotEmpty().WithMessage(_localizer[SharedResourcesKeys.NotEmpty])
                 .NotNull().WithMessage(_localizer[SharedResourcesKeys.Required]);
            RuleFor(x => x.ConfirmPassword)
                 .Equal(x => x.Password).WithMessage(_localizer[SharedResourcesKeys.PasswordNotEqualConfirmPass]);
            RuleFor(x => x.OrganizationName)
            .NotEmpty().WithMessage("??? ???????/??????? ?????") // ???? ?????? Localizer ???
              .When(x => x.UserType != UserType.IndividualUser); // ????? ??? ?? ?? ???? ????

            RuleFor(x => x.OrganizationType)
                .NotNull().WithMessage("??? ????? ??? ??????")
                .When(x => x.UserType != UserType.IndividualUser);

            RuleFor(x => x.OrganizationAddress)
                .NotEmpty().WithMessage("????? ?????? ?????")
                .When(x => x.UserType != UserType.IndividualUser);
        }

        public void ApplyCustomValidationsRules()
        {

        }
        #endregion
    }
}