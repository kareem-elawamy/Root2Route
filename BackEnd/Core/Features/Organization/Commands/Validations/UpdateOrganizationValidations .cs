using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Features.Organization.Commands.Models;
using FluentValidation;

namespace Core.Features.Organization.Commands.Validations
{
    public class UpdateOrganizationValidations : AbstractValidator<UpdateOrganizations>
    {
        public UpdateOrganizationValidations ()
        {
            RuleFor(x=>x.OrganizationId)
                .NotEmpty().WithMessage("OrganizationId is required");
            RuleFor(x=>x.OwnerId)
                .NotEmpty().WithMessage("OrganizationId is required");
             RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100);
              RuleFor(x => x.Description)
            .MaximumLength(500);
            
        }
    }
}