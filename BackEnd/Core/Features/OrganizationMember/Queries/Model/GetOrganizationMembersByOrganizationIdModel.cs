using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Features.OrganizationMember.Queries.Result;

namespace Core.Features.OrganizationMember.Queries.Model
{
    public class GetOrganizationMembersByOrganizationModel : IRequest<Response<List<GetOrganizationMembersByOrganizationIdResult>>>
    {
        public Guid OrganizationId { get; set; }
        public GetOrganizationMembersByOrganizationModel(Guid organizationId)
        {
            OrganizationId = organizationId;
        }
    }
}