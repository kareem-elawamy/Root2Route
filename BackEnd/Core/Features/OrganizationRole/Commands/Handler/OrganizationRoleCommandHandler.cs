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
    public class OrganizationRoleCommandHandler : ResponseHandler
    {
        private readonly IOrganizationRoleService _organizationRoleService;
        private readonly IMapper _mapper;
        // نفترض وجود خدمة للتحقق من الصلاحيات كما ناقشنا سابقاً
        private readonly IAuthorizationService _authService;

        public OrganizationRoleCommandHandler(IAuthorizationService authService, IOrganizationRoleService organizationRoleService, IMapper mapper)
        {
            _authService = authService;
            _mapper = mapper;
            _organizationRoleService = organizationRoleService;
        }

        // public async Task<Response<string>> Handle(AddOrganizationRoleCommand request, CancellationToken cancellationToken)
        // {
        //     var isAllowed = await _authService.HasPermissionAsync(
        //     request.RequesterUserId,
        //     request.OrganizationId,
        //     "Permissions.Employees.ManageRoles" // الصلاحية المطلوبة من ملف الثوابت
        // );

        //     if (!isAllowed) return Unauthorized<string>("You are not allowed to add roles to this organization.");



        //     // 2. Mapping
        //     var organizationRole = _mapper.Map<Domain.Models.OrganizationRole>(request);

        //     // 3. Execution
        //     var result = await _organizationRoleService.CreateOrganizationRole(organizationRole);

        //     if (result == "Exists")
        //     {
        //         return BadRequest<string>("Organization Role with the same name already exists.");
        //     }

        //     return Created(result);
        // }
   
    }
}