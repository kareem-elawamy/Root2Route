using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Features.Authentication.Commands.Validations
{
    public class ForgetPasswordValidations : AbstractValidator<ResetPasswordWithOtpCommand>
    {
        public ForgetPasswordValidations()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(6);
            RuleFor(x => x.Otp).NotEmpty().Length(6);
        }

    }
}