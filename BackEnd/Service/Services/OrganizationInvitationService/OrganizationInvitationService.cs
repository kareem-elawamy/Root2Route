using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enums;
using Infrastructure.Repositories.OrganizationInvitationRepository;
using Infrastructure.Repositories.OrganizationMemberRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Service.Services.OrganizationInvitationService
{
    public class OrganizationInvitationService : IOrganizationInvitationService
    {
        private readonly IOrganizationInvitationRepository _organizationInvitationRepository;
        private readonly IOrganizationMemberRepository _organizationMemberRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _dbContext;
        public OrganizationInvitationService(IOrganizationInvitationRepository organizationInvitationRepository,
            IOrganizationMemberRepository organizationMemberRepository,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _organizationMemberRepository = organizationMemberRepository;
            _organizationInvitationRepository = organizationInvitationRepository;
        }
        // 1. قبول دعوة جديدة
        public async Task<string> AcceptInvitationAsync(Guid invitationId, Guid userId)
        {
            var invitation = await _organizationInvitationRepository.GetByIdAsync(invitationId);

            if (invitation == null ||
                invitation.Status != InvitationStatus.Pending ||
                invitation.ExpiryDate < DateTime.UtcNow)
            {
                return "Failed: Invalid or expired invitation";
            }

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null ||
                !string.Equals(invitation.Email, user.Email, StringComparison.OrdinalIgnoreCase))
            {
                return "Failed: This invitation is not for the current user";
            }

            var isMember = await _organizationMemberRepository.GetTableNoTracking()
                .AnyAsync(m => m.OrganizationId == invitation.OrganizationId && m.UserId == userId);

            if (isMember)
                return "Failed: User is already a member";

            // 🔐 مهم: تحقق من صحة الـ Role
            if (invitation.Role == null || invitation.Role.OrganizationId != invitation.OrganizationId)
                return "Failed: Invalid role";

            using var transaction = await _organizationMemberRepository.BeginTransactionAsync();

            await _organizationMemberRepository.AddAsync(new OrganizationMember
            {
                Id = Guid.NewGuid(),
                OrganizationId = invitation.OrganizationId,
                UserId = userId,
                OrganizationRoleId = invitation.RoleId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });

            invitation.Status = InvitationStatus.Accepted;
            await _organizationInvitationRepository.UpdateAsync(invitation);

            await transaction.CommitAsync();

            return "Success";
        }
        // 2. إلغاء دعوة (رفضها)
        public async Task<string> RevokeInvitationAsync(Guid invitationId)
        {
            var invitation = await _organizationInvitationRepository.GetByIdAsync(invitationId);
            if (invitation == null || invitation.Status != InvitationStatus.Pending)
            {
                return "Failed: Invalid invitation";
            }

            invitation.Status = InvitationStatus.Rejected;
            await _organizationInvitationRepository.UpdateAsync(invitation);
            return "Success";
        }
        // 3. جلب كل الدعوات الخاصة بمنظمة معينة
        public async Task<List<OrganizationInvitation>> GetInvitationsByOrganizationIdAsync(Guid organizationId)
        {
            return await _organizationInvitationRepository.GetTableNoTracking()
                .Where(i => i.OrganizationId == organizationId)
                .ToListAsync();
        }

        // 4. إرسال دعوة جديدة
        public async Task<string> sendInvitation(Guid organizationId, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return "Failed: User not found";
            }
            await _organizationInvitationRepository.AddAsync(new OrganizationInvitation
            {
                Id = Guid.NewGuid(),
                OrganizationId = organizationId,
                Email = email,
                CreatedAt = DateTime.UtcNow,
                Status = InvitationStatus.Pending,
                ExpiryDate = DateTime.UtcNow.AddDays(7), // Set an expiry date for the invitation
                Token = Guid.NewGuid().ToString() // Generate a unique token for the invitation
            });
            return "Success";
        }
        // 5. جلب كل الدعوات الخاصة بمستخدم معين
        public async Task<string> GetAllInvitationsForUserAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return "Failed: User not found";
            }

            var invitations = await _organizationInvitationRepository.GetTableNoTracking()
                .Where(i => i.Email == user.Email && i.Status == InvitationStatus.Pending)
                .ToListAsync();

            return invitations.Count > 0 ? "Success" : "No pending invitations found";
        }
    }
}