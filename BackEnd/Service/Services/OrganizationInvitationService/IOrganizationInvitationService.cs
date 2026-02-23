using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Services.OrganizationInvitationService
{
    public interface IOrganizationInvitationService
    {
        Task<string> sendInvitation(Guid organizationId, string email);
        Task<List<OrganizationInvitation>> GetInvitationsByOrganizationIdAsync(Guid organizationId);
        Task<string> RevokeInvitationAsync(Guid invitationId);
        Task<string> AcceptInvitationAsync(Guid invitationId, Guid userId);
        Task<string> GetAllInvitationsForUserAsync(Guid userId);
    }
}