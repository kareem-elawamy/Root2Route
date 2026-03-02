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

        public async Task<string> AddOrganizationMemberAsync(OrganizationMember organizationMember)
        {
            var result = await _organizationMemberRepository.AddAsync(organizationMember);
            return result != null ? "Success" : "Failed";
        }

        public async Task<List<OrganizationMember>> GetOrganizationMembersByOrganizationIdAsync(Guid organizationId)
        {
            return await _organizationMemberRepository.GetOrganizationMembersByOrganizationIdAsync(organizationId);
        }
        public async Task<string> RemoveOrganizationMemberAsync(Guid organizationMemberId)
        {
            var member = await _organizationMemberRepository.GetByIdAsync(organizationMemberId);
            if (member == null) return "Failed: Member not found";
            await _organizationMemberRepository.DeleteAsync(member!);

            return "Success";
        }
        public async Task<string> UpdateOrganizationMemberRoleAsync(Guid organizationMemberId, Guid newRoleId)
        {
            var member = await _organizationMemberRepository.GetByIdAsync(organizationMemberId);
            if (member == null) return "Failed: Member not found";

            member.OrganizationRoleId = newRoleId;

            await _organizationMemberRepository.UpdateAsync(member);
            return "Success";
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
        public async Task<string> TransferOwnershipAsync(Guid organizationId, Guid newOwnerId)
        {
            var currentOwner = await _organizationMemberRepository.GetTableAsTracking()
                .Include(m => m.OrganizationRole)
                .FirstOrDefaultAsync(m => m.OrganizationId == organizationId && m.IsActive && m.OrganizationRole.Name == "Owner");

            if (currentOwner == null) return "Failed: Current owner not found";

            var newOwner = await _organizationMemberRepository.GetTableAsTracking()
                .FirstOrDefaultAsync(m => m.OrganizationId == organizationId && m.UserId == newOwnerId && m.IsActive);

            if (newOwner == null) return "Failed: New owner must be an active member";

            using var transaction = await _organizationMemberRepository.BeginTransactionAsync();
            try
            {
                var ownerRoleId = await GetRoleIdByNameAsync(organizationId, "Owner");
                var memberRoleId = await GetRoleIdByNameAsync(organizationId, "Member");

                newOwner.OrganizationRoleId = ownerRoleId;
                currentOwner.OrganizationRoleId = memberRoleId;

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
            var roleId = await _organizationMemberRepository.GetTableNoTracking()
                .Where(m => m.OrganizationId == organizationId && m.OrganizationRole.Name == roleName)
                .Select(m => m.OrganizationRoleId)
                .FirstOrDefaultAsync();


            return roleId ?? Guid.Empty;
        }

    }
}