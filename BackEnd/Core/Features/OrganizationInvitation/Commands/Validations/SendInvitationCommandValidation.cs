using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Features.OrganizationInvitation.Commands.Models;

namespace Core.Features.OrganizationInvitation.Commands.Validations
{
    public class SendInvitationCommandValidation : AbstractValidator<SendInvitationCommandModel>
    {
        public SendInvitationCommandValidation()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");
            RuleFor(x => x.OrganizationId)
                .NotEmpty().WithMessage("OrganizationId is required");
        }
    }
}