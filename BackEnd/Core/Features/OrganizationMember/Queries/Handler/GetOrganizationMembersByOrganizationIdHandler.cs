using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Features.OrganizationMember.Queries.Model;
using Core.Features.OrganizationMember.Queries.Result;
using Service.Services.OrganizationMemberService;

namespace Core.Features.OrganizationMember.Queries.Handler
{
    public class GetOrganizationMembersByOrganizationIdHandler : ResponseHandler
    , IRequestHandler<GetOrganizationMembersByOrganizationModel, Response<List<GetOrganizationMembersByOrganizationIdResult>>>
    {
        private readonly IOrganizationMemberService _organizationMember;
        private IMapper _mapper;
        public GetOrganizationMembersByOrganizationIdHandler(IMapper mapper, IOrganizationMemberService organizationMember)
        {
            _organizationMember = organizationMember;
            _mapper = mapper;
        }

        public async Task<Response<List<GetOrganizationMembersByOrganizationIdResult>>> Handle(GetOrganizationMembersByOrganizationModel request, CancellationToken cancellationToken)
        {
            var organizationMembers = await _organizationMember.GetOrganizationMembersByOrganizationIdAsync(request.OrganizationId);
            var result = _mapper.Map<List<GetOrganizationMembersByOrganizationIdResult>>(organizationMembers);
            return Success(result);
        }
        
    }
}