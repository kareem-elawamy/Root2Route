using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Services.OrganizationInvitationService
{
    public interface IOrganizationInvitationService
    {
        Task<InvitationResult> SendInvitationAsync(OrganizationInvitation invitation);
        Task<List<OrganizationInvitation>> GetInvitationsByOrganizationIdAsync(Guid organizationId);
        Task<InvitationResult> RevokeInvitationAsync(Guid invitationId, Guid userId);
        Task<InvitationResult> AcceptInvitationAsync(Guid invitationId, Guid userId);
        Task<List<OrganizationInvitation>> GetAllInvitationsForUserAsync(Guid userId);
    }
}