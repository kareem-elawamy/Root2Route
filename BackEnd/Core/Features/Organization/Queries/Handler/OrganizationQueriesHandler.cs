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
    public class OrganizationQueriesHandler : ResponseHandler,
   IRequestHandler<GetOrganizationsById, Response<OrganizationResponse>>,
   IRequestHandler<GetAllOrganizations, Response<List<OrganizationResponse>>>,
   IRequestHandler<GetAllOrganizationsByUserId, Response<List<OrganizationResponse>>>,
   IRequestHandler<GetOrganizationStatisticsQuery, Response<object>>
    {
        private readonly IOrganizationService _organizationService;
        private readonly IMapper _mapper;
        public OrganizationQueriesHandler(IMapper mapper, IOrganizationService organizationService)
        {
            _mapper = mapper;
            _organizationService = organizationService;
        }

        public async Task<Response<OrganizationResponse>> Handle(GetOrganizationsById request, CancellationToken cancellationToken)
        {
            var organization = await _organizationService.GetByIdAsync(request.Id);

            if (organization == null)
                return NotFound<OrganizationResponse>("Organization Not Found");

            var mapped = _mapper.Map<OrganizationResponse>(organization);

            return Success(mapped);
        }

        public async Task<Response<List<OrganizationResponse>>> Handle(GetAllOrganizations request, CancellationToken cancellationToken)
        {
            var organizations = await _organizationService.GetAllAsync();
            if (organizations == null)
                return NotFound<List<OrganizationResponse>>("Organizations Not Found");
            var mapped = _mapper.Map<List<OrganizationResponse>>(organizations);
            return Success(mapped);

        }

        public async Task<Response<List<OrganizationResponse>>> Handle(GetAllOrganizationsByUserId request, CancellationToken cancellationToken)
        {

            var organizations = await _organizationService.GetMyOrganizationsAsync(request.OwnerId);
            if (organizations == null)
                return NotFound<List<OrganizationResponse>>("Organizations Not Found");
            var mapped = _mapper.Map<List<OrganizationResponse>>(organizations);
            return Success(mapped);
        }

        public async Task<Response<object>> Handle(GetOrganizationStatisticsQuery request, CancellationToken cancellationToken)
        {
            var stats = await _organizationService
           .GetStatisticsAsync(request.OrganizationId);

            return Success(stats);
            throw new NotImplementedException();
        }

    }
}