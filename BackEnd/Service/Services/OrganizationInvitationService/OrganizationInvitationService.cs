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
        #region CRUD Operations
        #region Accept and Revoke Invitations
        public async Task<InvitationResult> AcceptInvitationAsync(Guid invitationId, Guid userId)
        {
            var invitation = await _organizationInvitationRepository.GetByIdAsync(invitationId);

            if (invitation == null)
                return InvitationResult.NotFound;

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
                return InvitationResult.AlreadyMember;
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

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

            return InvitationResult.Success;
        }
        public async Task<InvitationResult> RevokeInvitationAsync(Guid invitationId, Guid userId)
        {
            var invitation = await _organizationInvitationRepository.GetByIdAsync(invitationId);
            if (invitation == null || invitation.Status != InvitationStatus.Pending)
                return InvitationResult.NotFound;
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null ||
                !string.Equals(invitation.Email, user.Email, StringComparison.OrdinalIgnoreCase))
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