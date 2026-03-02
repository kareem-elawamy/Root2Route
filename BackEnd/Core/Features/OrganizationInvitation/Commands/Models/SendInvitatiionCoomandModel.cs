using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Service.Enums;

namespace Core.Features.OrganizationInvitation.Commands.Models
{
    public record SendInvitationCommandModel(
        Guid OrganizationId,
        Guid RoleId,
        string Email,
        Guid SenderId,
        DateTime ExpiryDate
    ) : IRequest<Response<string>>;
}