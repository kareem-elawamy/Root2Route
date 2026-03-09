    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Features.OrganizationRole.Queries.Models;
    using Core.Features.OrganizationRole.Queries.Result;
    using Service.Services.OrganizationRoleService;

    namespace Core.Features.OrganizationRole.Queries.Handler
    {
        public class GetOrganizationRolesByOrganizationIdQueries : ResponseHandler,
            IRequestHandler<GetOrganizationRolesByOrganizationIdModel, Response<List<GetOrganizationRolesByOrganizationIdResult>>>
        {
            private readonly IOrganizationRoleService _organizationRoleRepository;
            private readonly IMapper _mapper;
            public GetOrganizationRolesByOrganizationIdQueries(IOrganizationRoleService organizationRoleRepository, IMapper mapper)
            {
                _organizationRoleRepository = organizationRoleRepository;
                _mapper = mapper;
            }
            public async Task<Response<List<GetOrganizationRolesByOrganizationIdResult>>> Handle(GetOrganizationRolesByOrganizationIdModel request, CancellationToken cancellationToken)
            {
                var organizationRoles = await _organizationRoleRepository.GetOrganizationRolesAsyncByOrganizationId(request.OrganizationId);
                var mappedResult = _mapper.Map<List<GetOrganizationRolesByOrganizationIdResult>>(organizationRoles);
                return Success(mappedResult);
            }
        }
    }