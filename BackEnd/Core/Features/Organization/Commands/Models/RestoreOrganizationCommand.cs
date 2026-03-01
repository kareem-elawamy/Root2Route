using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Features.Organization.Commands.Models
{
    public record RestoreOrganizationCommand
  (
    Guid OrganizationId,
    Guid OwnerId
) : IRequest<Response<string>>;
}