using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Repositories.OrganizationMemberRepository;
using Microsoft.EntityFrameworkCore;

namespace Service.Services.OrganizationMemberService
{
    public class OrganizationMemberService : IOrganizationMemberService
    {
        private readonly IOrganizationMemberRepository _organizationMemberRepository;

        public OrganizationMemberService(IOrganizationMemberRepository organizationMemberRepository)
        {
            _organizationMemberRepository = organizationMemberRepository;
        }


        // 1. إضافة عضو جديد
        public async Task<string> AddOrganizationMemberAsync(OrganizationMember organizationMember)
        {
            var result = await _organizationMemberRepository.AddAsync(organizationMember);
            return result != null ? "Success" : "Failed";
        }
        // 2. جلب كل الأعضاء في منظمة معينة

        public async Task<List<OrganizationMember>> GetOrganizationMembersByOrganizationIdAsync(Guid organizationId)
        {
            return await _organizationMemberRepository.GetOrganizationMembersByOrganizationIdAsync(organizationId);
        }
        // 3. إزالة عضو من المنظمة
        public async Task<string> RemoveOrganizationMemberAsync(Guid organizationMemberId)
        {
            var member = await _organizationMemberRepository.GetByIdAsync(organizationMemberId);
            if (member == null) return "Failed: Member not found";
            await _organizationMemberRepository.DeleteAsync(member!);

            return "Success";
        }
        // 4. تحديث دور عضو في المنظمة
        public async Task<string> UpdateOrganizationMemberRoleAsync(Guid organizationMemberId, Guid newRoleId)
        {
            var member = await _organizationMemberRepository.GetByIdAsync(organizationMemberId);
            if (member == null) return "Failed: Member not found";

            member.OrganizationRoleId = newRoleId;

            await _organizationMemberRepository.UpdateAsync(member);
            return "Success";
        }
        // 5. تعطيل عضو في المنظمة
        public async Task<string> DeactivateOrganizationMemberAsync(Guid organizationMemberId)
        {
            var member = await _organizationMemberRepository.GetByIdAsync(organizationMemberId);
            if (member == null) return "Failed: Member not found";

            member.IsActive = false;
            await _organizationMemberRepository.UpdateAsync(member);
            return "Success";
        }
        // 6. تفعيل عضو في المنظمة
        public async Task<string> ActivateOrganizationMemberAsync(Guid organizationMemberId)
        {
            var member = await _organizationMemberRepository.GetByIdAsync(organizationMemberId);
            if (member == null) return "Failed: Member not found";

            member.IsActive = true;
            await _organizationMemberRepository.UpdateAsync(member);
            return "Success";
        }
        // 7. جلب عضو معين بالـ Id
        public async Task<OrganizationMember> GetOrganizationMemberByIdAsync(Guid organizationMemberId)
        {
            if (organizationMemberId == Guid.Empty) return null!;
            return await _organizationMemberRepository.GetByIdAsync(organizationMemberId).ConfigureAwait(false) ?? null!;
        }
        // 8. نقل ملكية المنظمة لعضو آخر
        public async Task<string> TransferOwnershipAsync(Guid organizationId, Guid newOwnerId)
        {
            // 1. استخدم Tracking هنا عشان التعديل يتم بسهولة
            var currentOwner = await _organizationMemberRepository.GetTableAsTracking()
                .Include(m => m.OrganizationRole)
                .FirstOrDefaultAsync(m => m.OrganizationId == organizationId && m.IsActive && m.OrganizationRole.Name == "Owner");

            if (currentOwner == null) return "Failed: Current owner not found";

            var newOwner = await _organizationMemberRepository.GetTableAsTracking()
                .FirstOrDefaultAsync(m => m.OrganizationId == organizationId && m.UserId == newOwnerId && m.IsActive);

            if (newOwner == null) return "Failed: New owner must be an active member";

            // 2. استخدام Transaction لضمان إن الدورين يتغيروا مع بعض أو لا
            using var transaction = await _organizationMemberRepository.BeginTransactionAsync();
            try
            {
                var ownerRoleId = await GetRoleIdByNameAsync(organizationId, "Owner");
                var memberRoleId = await GetRoleIdByNameAsync(organizationId, "Member");

                newOwner.OrganizationRoleId = ownerRoleId;
                currentOwner.OrganizationRoleId = memberRoleId;

                // التحديث هيسمع تلقائيا لأننا شغالين Tracking
                await _organizationMemberRepository.UpdateAsync(currentOwner);
                await _organizationMemberRepository.UpdateAsync(newOwner);

                await transaction.CommitAsync();
                return "Success";
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return "Failed: An error occurred during transfer";
            }
        }
        // دالة مساعدة لجلب الـ RoleId بناءً على اسم الدور
        private async Task<Guid> GetRoleIdByNameAsync(Guid organizationId, string roleName)
        {
            var roleId = await _organizationMemberRepository.GetTableNoTracking()
                .Where(m => m.OrganizationId == organizationId && m.OrganizationRole.Name == roleName)
                .Select(m => m.OrganizationRoleId)
                .FirstOrDefaultAsync();


            return roleId ?? Guid.Empty;
        }
        // ملاحظة: في حالة وجود Roles متعددة بنفس الاسم (غير متوقع)، الدالة هترجع أول واحد بس. ممكن تضيف تحقق إضافي لو حبيت.
        
    }
}