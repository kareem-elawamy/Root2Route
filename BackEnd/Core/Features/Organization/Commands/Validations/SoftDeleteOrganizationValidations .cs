using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Core.Features.Organization.Commands.Validations

{
    public class SoftDeleteOrganizationValidations
  : AbstractValidator<SoftDeleteOrganizationCommand>
    {
        public SoftDeleteOrganizationValidations()
        {
            RuleFor(x => x.OrganizationId)
                .NotEmpty();

            RuleFor(x => x.OwnerId)
                .NotEmpty();
        }
    }
}