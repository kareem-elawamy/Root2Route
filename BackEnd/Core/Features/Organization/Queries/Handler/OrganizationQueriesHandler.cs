using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Base;
using Core.Features.Organization.Queries.Models;
using Core.Features.Organization.Queries.Result;
using MediatR;
using Service.Services;

namespace Core.Features.Organization.Queries.Handler
{
    public class OrganizationQueriesHandler : ResponseHandler
    //,  IRequestHandler<GetAllOwnerOrganizations, Response<List<OrganizationResponse>>>
    {
        private readonly IOrganizationService _organizationService;
        private readonly IMapper _mapper;
        public OrganizationQueriesHandler(IMapper mapper, IOrganizationService organizationService)
        {
            _mapper = mapper;
            _organizationService = organizationService;
        }
        // public async Task<Response<List<OrganizationResponse>>> Handle(GetAllOwnerOrganizations request, CancellationToken cancellationToken)
        // {
        //     var organizations = await _organizationService.GetAllOnwerOrganizationsAsync(request.OwnerId);
        //     var mappedOrganizations = _mapper.Map<List<OrganizationResponse>>(organizations);
        //     return Success(mappedOrganizations);
        // }
    }
}