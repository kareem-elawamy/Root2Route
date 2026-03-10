using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Base;
using Core.Features.OrganizationRole.Commands.Models;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Service.Services.AuthorizationService;
using Service.Services.OrganizationRoleService;

namespace Core.Features.OrganizationRole.Commands.Handler
{
    public class OrganizationRoleCommandHandler : ResponseHandler,
        IRequestHandler<AddOrganizationRoleCommand, Response<string>>
    {
        private readonly IOrganizationRoleService _organizationRoleService;
        private readonly IMapper _mapper;
        private readonly IAuthorizationService _authService;

        public OrganizationRoleCommandHandler(IAuthorizationService authService, IOrganizationRoleService organizationRoleService, IMapper mapper)
        {
            _authService = authService;
            _mapper = mapper;
            _organizationRoleService = organizationRoleService;
        }

        public async Task<Response<string>> Handle(AddOrganizationRoleCommand request, CancellationToken cancellationToken)
        {

            var newRole = new Domain.Models.OrganizationRole
            {
                Id = Guid.NewGuid(),
                OrganizationId = request.OrganizationId,
                Name = request.Name,
                IsSystemDefault = false,
                Permissions = request.Permissions.Select(p => new OrganizationRolePermission
                {
                    Id = Guid.NewGuid(),
                    PermissionsClaim = p
                }).ToList()
            };

            var result = await _organizationRoleService.CreateOrganizationRole(newRole);

            if (result == "exists")
                return BadRequest<string>("Failed: A role with this name already exists in your organization.");

            return Success<string>("Role created successfully");
        }
    }
}