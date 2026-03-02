using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Features.Organization.Commands.Validations
{
    public class UploadOrganizationLogoValidations
     : AbstractValidator<UploadOrganizationLogoCommand>
    {
        public UploadOrganizationLogoValidations()
        {
            RuleFor(x => x.OrganizationId)
                .NotEmpty();

            RuleFor(x => x.OwnerId)
                .NotEmpty();

            RuleFor(x => x.File)
                .NotNull().WithMessage("Logo file is required");

            RuleFor(x => x.File.Length)
                .LessThanOrEqualTo(2 * 1024 * 1024)
                .WithMessage("Logo must not exceed 2MB");

            RuleFor(x => x.File.ContentType)
                .Must(type => type == "image/png" || type == "image/jpeg")
                .WithMessage("Only PNG and JPEG are allowed");
        }
    }
}