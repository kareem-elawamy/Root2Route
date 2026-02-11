using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Features.Organization.Commands.Models;
using FluentValidation;

namespace Core.Features.Organization.Commands.Validations
{
    public class CreateOrganizationValidations : AbstractValidator<CreateOrganizationCommand>
    {

        public CreateOrganizationValidations()
        {
            RuleFor(x => x.Name)
                 .NotEmpty().WithMessage("Name is required.")
                 .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

            RuleFor(x => x.ContactEmail)
                .EmailAddress().WithMessage("Invalid email format.")
                .MaximumLength(100).WithMessage("Contact email cannot exceed 100 characters.");

            RuleFor(x => x.ContactPhone)
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format.")
                .MaximumLength(15).WithMessage("Contact phone cannot exceed 15 characters.");
            RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid organization type.");
            RuleFor(x => x.OwnerId)
                .NotEmpty().WithMessage("OwnerId is required.")
                .Must(id => Guid.TryParse(id.ToString(), out _)).WithMessage("OwnerId must be a valid GUID.");
        }
    }
}