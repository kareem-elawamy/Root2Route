using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Features.OrganizationRole.Queries.Result;

namespace Core.Features.OrganizationRole.Queries.Models
{
    public class GetOrganizationRolesByOrganizationIdModel : IRequest<Response<List<GetOrganizationRolesByOrganizationIdResult>>>
    {
        public Guid OrganizationId { get; set; }
        public GetOrganizationRolesByOrganizationIdModel(Guid organizationId)
        {
            OrganizationId = organizationId;
        }
    }
}