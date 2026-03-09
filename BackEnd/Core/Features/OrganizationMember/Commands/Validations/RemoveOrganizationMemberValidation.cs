using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Features.OrganizationMember.Commands.Models;

namespace Core.Features.OrganizationMember.Commands.Validations
{
    public class RemoveOrganizationMemberValidation : AbstractValidator<RemoveOrganizationMemberModel>
    {

        public RemoveOrganizationMemberValidation()
        {
            RuleFor(x => x.OrganizationMemberId)
                .NotEmpty().WithMessage("Organization Member ID is required.")
                .Must(id => Guid.TryParse(id.ToString(), out _)).WithMessage("Invalid Organization Member ID format.");
        }
        
    }
}