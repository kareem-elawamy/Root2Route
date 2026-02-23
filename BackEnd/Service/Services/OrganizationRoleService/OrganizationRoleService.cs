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

        // 1. إنشاء دور جديد في المنظمة
        public async Task<string> CreateOrganizationRole(OrganizationRole organizationRole)
        {
            var exists = await _organizationRoleRepository.GetTableNoTracking()
         .AnyAsync(x => x.Name == organizationRole.Name && x.OrganizationId == organizationRole.OrganizationId);
            if (exists) return "exists";
            await _organizationRoleRepository.AddAsync(organizationRole);
            return "Success";
        }
        // 2. جلب كل الأدوار في منظمة معينة
        public async Task<List<OrganizationRole>> GetOrganizationRolesAsyncByOrganizationId(Guid OrganizationId)
        {
            return await _organizationRoleRepository.GetTableNoTracking()
                                                    .Where(x => !x.IsDeleted && x.OrganizationId == OrganizationId)
                                                    .OrderByDescending(x => x.CreatedAt)
                                                    .ToListAsync();
        }
        // 3. حذف دور من المنظمة (تعيين IsDeleted = true)
        public async Task<string> DeleteOrganizationRole(Guid organizationRoleId)
        {
            var organizationRole = await _organizationRoleRepository.GetByIdAsync(organizationRoleId);
            if (organizationRole == null) return "NotFound";
            organizationRole.IsDeleted = true;
            await _organizationRoleRepository.UpdateAsync(organizationRole);
            return "Success";
        }
        // 4. تحديث اسم وصلاحيات دور في المنظمة
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