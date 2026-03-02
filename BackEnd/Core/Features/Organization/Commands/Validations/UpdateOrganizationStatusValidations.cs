using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Features.Organization.Commands.Validations
{
    public class UpdateOrganizationStatusValidations
     : AbstractValidator<UpdateOrganizationStatusCommand>
    {
        public UpdateOrganizationStatusValidations()
        {
            RuleFor(x => x.OrganizationId)
                .NotEmpty();

            RuleFor(x => x.NewStatus)
                .IsInEnum();
        }
    }
}