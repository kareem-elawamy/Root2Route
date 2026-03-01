using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enums;

namespace Core.Features.Organization.Commands.Models
{
    public record UpdateOrganizationStatusCommand(
    Guid OrganizationId,
    OrganizationStatus NewStatus
) : IRequest<Response<string>>;
}