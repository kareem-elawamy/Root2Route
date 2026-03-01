using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Features.Organization.Queries.Models
{
    public record GetOrganizationStatisticsQuery(
    Guid OrganizationId
) : IRequest<Response<object>>;
}