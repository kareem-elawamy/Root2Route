using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Features.OrganizationInvitation.Commands.Models
{
    public record AcceptInvitationCommandModel(
     Guid UserId,
     Guid InvitationId
    ) : IRequest<Response<string>>;
}