using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Repositories.OrganizationRoleRepository;
using Microsoft.EntityFrameworkCore;


namespace Service.Services.OrganizationRoleService
{
    public class OrganizationRoleService : IOrganizationRoleService
    {
        private readonly IOrganizationRoleRepository _organizationRoleRepository;
        public OrganizationRoleService(IOrganizationRoleRepository organizationRoleRepository)
        {
            _organizationRoleRepository = organizationRoleRepository;

        }

        public async Task<string> CreateOrganizationRole(OrganizationRole organizationRole)
        {
            var exists = await _organizationRoleRepository.GetTableNoTracking()
        .AnyAsync(x => x.Name == organizationRole.Name && x.OrganizationId == organizationRole.OrganizationId && !x.IsDeleted);
            if (exists) return "exists";
            await _organizationRoleRepository.AddAsync(organizationRole);
            return "Success";
        }
        public async Task<List<OrganizationRole>> GetOrganizationRolesAsyncByOrganizationId(Guid OrganizationId)
        {
            return await _organizationRoleRepository.GetTableNoTracking()
                                                    .Where(x => !x.IsDeleted && x.OrganizationId == OrganizationId)
                                                    .OrderByDescending(x => x.CreatedAt)
                                                    .ToListAsync();
        }
        public async Task<string> DeleteOrganizationRole(Guid organizationRoleId)
        {
            var organizationRole = await _organizationRoleRepository.GetByIdAsync(organizationRoleId);
            if (organizationRole == null) return "NotFound";
            organizationRole.IsDeleted = true;
            await _organizationRoleRepository.UpdateAsync(organizationRole);
            return "Success";
        }
        public async Task<string> UpdateOrganizationRole(OrganizationRole organizationRole)
        {
            var existingRole = await _organizationRoleRepository.GetByIdAsync(organizationRole.Id);
            if (existingRole == null) return "NotFound";
            existingRole.Name = organizationRole.Name;
            existingRole.Permissions = organizationRole.Permissions;
            await _organizationRoleRepository.UpdateAsync(existingRole);
            return "Success";
        }

    }
}