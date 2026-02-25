using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Features.Organization.Commands.Models
{
    public record ChangeOwnerCommand(
     Guid OrganizationId,
     Guid CurrentOwnerId,
     Guid NewOwnerId
 ) : IRequest<Response<string>>;
}