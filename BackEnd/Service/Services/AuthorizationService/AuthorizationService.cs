using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Repositories.OrganizationMemberRepository;
using Infrastructure.Repositories.OrganizationRepository;
using Microsoft.EntityFrameworkCore;

namespace Service.Services.AuthorizationService
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly IOrganizationRepository _orgRepo;
        private readonly IOrganizationMemberRepository _memberRepo;
        public AuthorizationService(
            IOrganizationRepository orgRepo,
            IOrganizationMemberRepository memberRepo)
        {
            _orgRepo = orgRepo;
            _memberRepo = memberRepo;
        }
        // public async Task<bool> HasPermissionAsync(Guid userId, Guid organizationId, string permission)
        // {
        //     var isOwner = await _orgRepo.GetTableNoTracking()
        //                     .AnyAsync(x => x.Id == organizationId && x.OwnerId == userId);

        //     if (isOwner) return true;
        //     var hasPermission = await _memberRepo.GetTableNoTracking()
        //         .Where(m => m.UserId == userId && m.OrganizationId == organizationId) // هات الموظف في الشركة دي
        //         .Where(m => m.IsActive) // تأكد إنه مش مفصول (Active)
        //         .SelectMany(m => m.OrganizationRole!.Permissions) // ادخل جوه الدور وهات الصلاحيات
        //         .AnyAsync(p => p.PermissionsClaim == permission); // هل الصلاحية المطلوبة موجودة؟
        //     return hasPermission;
        // }
    }
}