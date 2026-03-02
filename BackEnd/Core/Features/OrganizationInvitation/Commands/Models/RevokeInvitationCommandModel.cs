using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Features.OrganizationInvitation.Commands.Models
{
    public record RevokeInvitationCommandModel(
        Guid UserId,
        Guid InvitationId
    ) : IRequest<Response<string>>;
}