using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Repositories.OrganizationMemberRepository;
using Infrastructure.Repositories.OrganizationRoleRepository;
using Microsoft.EntityFrameworkCore;

namespace Service.Services.OrganizationMemberService
{
    public class OrganizationMemberService : IOrganizationMemberService
    {
        private readonly IOrganizationMemberRepository _organizationMemberRepository;
        private readonly IOrganizationRoleRepository _roleRepository;


        public OrganizationMemberService(IOrganizationRoleRepository organizationRole
            , IOrganizationMemberRepository organizationMemberRepository)
        {
            _roleRepository = organizationRole;
            _organizationMemberRepository = organizationMemberRepository;
        }

        public async Task<string> AddOrganizationMemberAsync(OrganizationMember organizationMember)
        {
            var result = await _organizationMemberRepository.AddAsync(organizationMember);
            return result != null ? "Success" : "Failed";
        }

        public async Task<List<OrganizationMember>> GetOrganizationMembersByOrganizationIdAsync(Guid organizationId)
        {
            var members = await _organizationMemberRepository.GetOrganizationMembersByOrganizationIdAsync(organizationId);

            if (members == null || !members.Any()) return new List<OrganizationMember>();
            if (members.Count == 1 && members[0].UserId == Guid.Empty) return new List<OrganizationMember>();
            Console.WriteLine($"Retrieved {members.Count} members for organization {organizationId}");
            return members;
        }
        public async Task<string> RemoveOrganizationMemberAsync(Guid organizationMemberId, Guid currentUserId)
        {
            var memberCheck = await _organizationMemberRepository.GetByIdAsync(organizationMemberId);
            if (memberCheck == null) return "Failed: Member not found";
            
            var isCallerAuthorized = await _organizationMemberRepository.GetTableNoTracking()
                .AnyAsync(m => m.OrganizationId == memberCheck.OrganizationId && m.UserId == currentUserId && m.OrganizationRoles.Any(r => r.Name == "Owner" || r.Name == "Admin"));
            if (!isCallerAuthorized) return "Failed: Unauthorized to remove members in this organization.";

            using var transaction = await _organizationMemberRepository.BeginTransactionAsync();
            try
            {
                // Console.WriteLine($"Attempting to remove organization member with ID: {organizationMemberId}");
                var member = await _organizationMemberRepository.GetByIdAsync(organizationMemberId);
                if (member == null) return "Failed: Member not found";
                if (member.IsDeleted) return "Failed: Member is already deleted";
                var ownerRoleId = await GetRoleIdByNameAsync(member.OrganizationId, "Owner");
                if (member.OrganizationRoles.Any(r => r.Id == ownerRoleId))
                {
                    return "Failed: Cannot remove an owner. Please transfer ownership before removing.";
                }
                member.IsDeleted = true;
                member.UpdatedAt = DateTime.UtcNow;
                member.OrganizationRoles.Clear();
                await _organizationMemberRepository.UpdateAsync(member);
                await _organizationMemberRepository.UpdateAsync(member!);
                await transaction.CommitAsync();
                return "Success";
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return "Failed: An error occurred while removing the organization member";
            }
        }
        public async Task<string> UpdateOrganizationMemberRoleAsync(Guid organizationMemberId, Guid newRoleId)
        {
            using var transaction = await _organizationMemberRepository.BeginTransactionAsync();
            try
            {

                var member = await _organizationMemberRepository.GetByIdAsync(organizationMemberId);
                if (member == null) return "Failed: Member not found";
                var assignedRole = await _roleRepository.GetByIdAsync(newRoleId);
                if (assignedRole == null) return "Failed: Role not found";
                if (assignedRole.OrganizationId != member.OrganizationId) return "Failed: Cross-tenant role assignment blocked";
                member.OrganizationRoles.Add(assignedRole);
                await _organizationMemberRepository.UpdateAsync(member);
                await transaction.CommitAsync();

                return "Success";
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return "Failed: An error occurred while updating the member's role";
            }
        }
        public async Task<string> DeactivateOrganizationMemberAsync(Guid organizationMemberId)
        {
            var member = await _organizationMemberRepository.GetByIdAsync(organizationMemberId);
            if (member == null) return "Failed: Member not found";

            member.IsActive = false;
            await _organizationMemberRepository.UpdateAsync(member);
            return "Success";
        }
        public async Task<string> ActivateOrganizationMemberAsync(Guid organizationMemberId)
        {
            var member = await _organizationMemberRepository.GetByIdAsync(organizationMemberId);
            if (member == null) return "Failed: Member not found";

            member.IsActive = true;
            await _organizationMemberRepository.UpdateAsync(member);
            return "Success";
        }
        public async Task<OrganizationMember> GetOrganizationMemberByIdAsync(Guid organizationMemberId)
        {
            if (organizationMemberId == Guid.Empty) return null!;
            return await _organizationMemberRepository.GetByIdAsync(organizationMemberId).ConfigureAwait(false) ?? null!;
        }
        public async Task<string> TransferOwnershipAsync(Guid organizationId, Guid newOwnerId, Guid currentOwnerId)
        {
            var ownerRoleId = await GetRoleIdByNameAsync(organizationId, "Owner");
            var currentOwner = await _organizationMemberRepository.GetTableAsTracking()
                .FirstOrDefaultAsync(m => m.OrganizationId == organizationId && m.UserId == currentOwnerId && m.IsActive && m.OrganizationRoles.Any(r => r.Id == ownerRoleId));

            if (currentOwner == null) return "Failed: Current owner not found";

            var newOwner = await _organizationMemberRepository.GetTableAsTracking()
                .FirstOrDefaultAsync(m => m.OrganizationId == organizationId && m.UserId == newOwnerId && m.IsActive);

            if (newOwner == null) return "Failed: New owner must be an active member";

            using var transaction = await _organizationMemberRepository.BeginTransactionAsync();
            try
            {
                ownerRoleId = await GetRoleIdByNameAsync(organizationId, "Owner");
                var memberRoleId = await GetRoleIdByNameAsync(organizationId, "Member");
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
        private async Task<Guid> GetRoleIdByNameAsync(Guid organizationId, string roleName)
        {
            var role = await _roleRepository.GetTableNoTracking()
                .Where(r => r.OrganizationId == organizationId && r.Name == roleName)
                .FirstOrDefaultAsync();
            return role != null ? role.Id : Guid.Empty;
        }

    }
}