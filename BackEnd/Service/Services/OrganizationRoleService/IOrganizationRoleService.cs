using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Services.OrganizationRoleService
{
    public interface IOrganizationRoleService
    {
        public Task<string> CreateOrganizationRole(OrganizationRole organizationRole);
        public Task<List<OrganizationRole>> GetOrganizationRolesAsyncByOrganizationId(Guid OrganizationId);
        public Task<string> DeleteOrganizationRole(Guid organizationRoleId);
        public Task<string> UpdateOrganizationRole(OrganizationRole organizationRole);
        
        
    }
}