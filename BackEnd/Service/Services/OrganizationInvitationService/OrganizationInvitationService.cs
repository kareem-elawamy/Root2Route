using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enums;
using Infrastructure.Repositories.OrganizationInvitationRepository;
using Infrastructure.Repositories.OrganizationMemberRepository;
using Infrastructure.Repositories.OrganizationRoleRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Service.Services.OrganizationInvitationService
{
    public class OrganizationInvitationService : IOrganizationInvitationService
    {
        private readonly IOrganizationInvitationRepository _organizationInvitationRepository;
        private readonly IOrganizationMemberRepository _organizationMemberRepository;
        private readonly IOrganizationRoleRepository _roleRepository;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _dbContext;
        public OrganizationInvitationService(IOrganizationRoleRepository organizationRole, IOrganizationInvitationRepository organizationInvitationRepository,
            IOrganizationMemberRepository organizationMemberRepository,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleRepository = organizationRole;
            _organizationMemberRepository = organizationMemberRepository;
            _organizationInvitationRepository = organizationInvitationRepository;
        }
        #region CRUD Operations
        #region Accept and Revoke Invitations
        public async Task<InvitationResult> AcceptInvitationAsync(Guid invitationId, Guid userId, string token)
        {
            var invitation = await _organizationInvitationRepository.GetByIdAsync(invitationId);

            if (invitation == null)
                return InvitationResult.NotFound;
            if (string.IsNullOrEmpty(token) || invitation.Token != token)
                return InvitationResult.Failed;

            if (invitation.Status != InvitationStatus.Pending ||
                invitation.ExpiryDate < DateTime.UtcNow)
                return InvitationResult.Expired;

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null ||
                !string.Equals(invitation.Email, user.Email, StringComparison.OrdinalIgnoreCase))
                return InvitationResult.InvalidUser;

            var isMember = await _organizationMemberRepository.GetTableNoTracking()
                .AnyAsync(m => m.OrganizationId == invitation.OrganizationId && m.UserId == userId);

            if (isMember)
            {
                var member = await _organizationMemberRepository.GetTableNoTracking()
                     .FirstOrDefaultAsync(m => m.OrganizationId == invitation.OrganizationId && m.UserId == userId);
                if (member!.IsDeleted || !member.IsActive)
                {
                    member.IsActive = true;
                    member.UpdatedAt = DateTime.UtcNow;
                    member.IsDeleted = false;
                    await _organizationMemberRepository.UpdateAsync(member);
                }
                invitation.Status = InvitationStatus.Accepted;
                await _organizationInvitationRepository.UpdateAsync(invitation);
                return InvitationResult.Success;
            }



            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var assignedRole = await _roleRepository.GetByIdAsync(invitation.RoleId);

                if (assignedRole == null)
                {
                    await transaction.RollbackAsync();
                    return InvitationResult.InvalidRole;
                }

                var newMember = new OrganizationMember
                {
                    Id = Guid.NewGuid(),
                    OrganizationId = invitation.OrganizationId,
                    UserId = userId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    OrganizationRoles = new List<OrganizationRole> { assignedRole } // التعديل الأهم هنا
                };

                await _organizationMemberRepository.AddAsync(newMember);

                invitation.Status = InvitationStatus.Accepted;
                await _organizationInvitationRepository.UpdateAsync(invitation);

                await transaction.CommitAsync();

                return InvitationResult.Success;
            }
            catch
            {
                await transaction.RollbackAsync();
                return InvitationResult.Failed; // أو أي Error Type بتستخدمه
            }
        }
        public async Task<InvitationResult> RevokeInvitationAsync(Guid invitationId, Guid userId)
        {
            var invitation = await _organizationInvitationRepository.GetByIdAsync(invitationId);
            if (invitation == null || invitation.Status != InvitationStatus.Pending)
                return InvitationResult.NotFound;
            
            var user = await _userManager.FindByIdAsync(userId.ToString());
            bool isRecipient = user != null && string.Equals(invitation.Email, user.Email, StringComparison.OrdinalIgnoreCase);
            bool isSenderOrAdmin = invitation.SenderId == userId;

            if (!isRecipient && !isSenderOrAdmin)
                return InvitationResult.InvalidUser;

            invitation.Status = InvitationStatus.Rejected;
            await _organizationInvitationRepository.UpdateAsync(invitation);
            return InvitationResult.Success;
        }
        #endregion
        public async Task<List<OrganizationInvitation>> GetInvitationsByOrganizationIdAsync(Guid organizationId)
        {
            return await _organizationInvitationRepository.GetTableNoTracking()
                .Include(i => i.Role)
                .Where(i => i.OrganizationId == organizationId && i.Status == InvitationStatus.Pending)
                .OrderBy(i => i.Status).
                ThenByDescending(i => i.CreatedAt)
                .ToListAsync();
        }
        public async Task<InvitationResult> SendInvitationAsync(OrganizationInvitation invitation)
        {
            var isCallerAuthorized = await _organizationMemberRepository.GetTableNoTracking()
                .AnyAsync(m => m.OrganizationId == invitation.OrganizationId && m.UserId == invitation.SenderId && m.OrganizationRoles.Any(r => r.Name == "Owner" || r.Name == "Admin"));
            if (!isCallerAuthorized) return InvitationResult.Failed;

            var user = await _userManager.FindByEmailAsync(invitation.Email);
            if (user == null)
                return InvitationResult.InvalidUser;

            var roleExists = await _dbContext.OrganizationRoles
                .AnyAsync(r => r.Id == invitation.RoleId && r.OrganizationId == invitation.OrganizationId);

            if (!roleExists)
                return InvitationResult.InvalidRole;

            var alreadyInvited = await _organizationInvitationRepository.GetTableNoTracking()
                .AnyAsync(i => i.OrganizationId == invitation.OrganizationId &&
                               i.Email == invitation.Email &&
                               i.Status == InvitationStatus.Pending);

            if (alreadyInvited)
                return InvitationResult.AlreadyInvited;
            invitation.Token = Guid.NewGuid().ToString();
            invitation.Status = InvitationStatus.Pending;
            await _organizationInvitationRepository.AddAsync(invitation);
            return InvitationResult.Success;
        }
        public async Task<List<OrganizationInvitation>> GetAllInvitationsForUserAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return new List<OrganizationInvitation>();

            return await _organizationInvitationRepository
                .GetTableNoTracking()
                .Include(i => i.Organization)
                .Include(i => i.Role)
                .Where(i => i.Email == user.Email && i.Status == InvitationStatus.Pending)
                .OrderBy(i => i.Status).
                ThenByDescending(i => i.CreatedAt)
                .ToListAsync();
        }
        #endregion
    }
}