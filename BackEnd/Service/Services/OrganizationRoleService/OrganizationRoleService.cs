using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Repositories.OrganizationRoleRepository;
using Microsoft.EntityFrameworkCore;
using Domain.Models; // تأكد من مسار الـ Models

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
            var roles = await _organizationRoleRepository.GetTableNoTracking()
                .Include(x => x.Permissions) // التعديل الأول: جلب الصلاحيات مع كل دور
                .Where(x => x.OrganizationId == OrganizationId && !x.IsDeleted) // ضفتلك فحص IsDeleted عشان ميجيبش الممسوح
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            Console.WriteLine($"Retrieved {roles.Count} roles for organization {OrganizationId}");
            return roles;
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
            // التعديل الثاني الأهم: استخدام استعلام بيعمل Include للصلاحيات القديمة عشان نعرف نعدلها
            var existingRole = await _organizationRoleRepository.GetTableAsTracking()
                .Include(r => r.Permissions)
                .FirstOrDefaultAsync(r => r.Id == organizationRole.Id);

            if (existingRole == null) return "NotFound";

            // تحديث الاسم
            existingRole.Name = organizationRole.Name;

            // تحديث الصلاحيات بأمان (يمسح القديم ويضيف الجديد اللي جاي من الريكويست)
            existingRole.Permissions.Clear();

            if (organizationRole.Permissions != null && organizationRole.Permissions.Any())
            {
                foreach (var permission in organizationRole.Permissions)
                {
                    existingRole.Permissions.Add(new OrganizationRolePermission
                    {
                        Id = Guid.NewGuid(),
                        PermissionsClaim = permission.PermissionsClaim,
                        OrganizationRoleId = existingRole.Id
                    });
                }
            }

            await _organizationRoleRepository.UpdateAsync(existingRole);
            return "Success";
        }
    }
}