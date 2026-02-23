using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Services.OrganizationAuthorizationService
{
    public class OrganizationAuthorizationService : IOrganizationAuthorizationService
    {
        public Task<bool> HasPermissionAsync(Guid userId, Guid organizationId, OrganizationRolePermission permission)
        {
            throw new NotImplementedException();
        }
    }
}