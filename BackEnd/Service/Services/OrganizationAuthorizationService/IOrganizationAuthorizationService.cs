using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Services.OrganizationAuthorizationService
{
    public interface IOrganizationAuthorizationService
    {
        Task<bool> HasPermissionAsync(Guid userId,
                               Guid organizationId,
                               OrganizationRolePermission permission);
    }
}