using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.OrganizationRolePermissionRepository
{
    public class OrganizationRolePermissionRepository : GenericRepositoryAsync<OrganizationRolePermission>, IOrganizationRolePermissionRepository
    {
        public OrganizationRolePermissionRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            
        }
    }
}