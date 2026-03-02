using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Features.Organization.Commands.Models;
using FluentValidation;

namespace Core.Features.Organization.Commands.Validations
{
    public class ChangeOwnerValidations
    : AbstractValidator<ChangeOwnerCommand>
    {
        public ChangeOwnerValidations()
        {
            RuleFor(x => x.OrganizationId)
                .NotEmpty();

            RuleFor(x => x.CurrentOwnerId)
                .NotEmpty();

            RuleFor(x => x.NewOwnerId)
                .NotEmpty()
                .NotEqual(x => x.CurrentOwnerId)
                .WithMessage("New owner must be different from current owner");
        }
    }
}