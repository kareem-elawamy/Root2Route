using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Infrastructure.Data;
using Infrastructure.Repositories.OrganizationMemberRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Filters
{
    public class PermissionAuthorizationFilter : IAsyncAuthorizationFilter
    {
        private readonly string _permission;

        // شيلنا الـ DbContext من الـ Constructor خالص
        public PermissionAuthorizationFilter(string permission)
        {
            _permission = permission;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var userIdClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)
                           ?? context.HttpContext.User.FindFirst("uid");

            if (userIdClaim == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var userId = Guid.Parse(userIdClaim.Value);

            if (!context.HttpContext.Request.Headers.TryGetValue("X-Organization-Id", out var orgIdString) ||
                !Guid.TryParse(orgIdString, out var organizationId))
            {
                context.Result = new BadRequestObjectResult("Organization Id is missing in headers.");
                return;
            }

            var dbContext = context.HttpContext.RequestServices.GetRequiredService<ApplicationDbContext>();

            var hasPermission = await dbContext.OrganizationMembers
                .Where(m => m.UserId == userId && m.OrganizationId == organizationId && !m.IsDeleted && m.IsActive)
                .SelectMany(m => m.OrganizationRoles)
                .SelectMany(r => r.Permissions)
                .AnyAsync(p => p.PermissionsClaim == _permission);

            if (!hasPermission)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}