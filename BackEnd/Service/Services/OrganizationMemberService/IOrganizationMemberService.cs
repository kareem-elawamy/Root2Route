using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Services.OrganizationMemberService
{
    public interface IOrganizationMemberService
    {
        Task<string> AddOrganizationMemberAsync(OrganizationMember organizationMember);
        Task<List<OrganizationMember>> GetOrganizationMembersByOrganizationIdAsync(Guid organizationId);
        Task<string> RemoveOrganizationMemberAsync(Guid organizationMemberId);
        Task<string> UpdateOrganizationMemberRoleAsync(Guid organizationMemberId, Guid newRoleId);
        Task<string> DeactivateOrganizationMemberAsync(Guid organizationMemberId);
        Task<string> ActivateOrganizationMemberAsync(Guid organizationMemberId);
        Task<OrganizationMember> GetOrganizationMemberByIdAsync(Guid organizationMemberId);
        Task<string> TransferOwnershipAsync(Guid organizationId, Guid newOwnerId);
    }
}