using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.OrganizationMemberRepository
{
    public interface IOrganizationMemberRepository : IGenericRepositoryAsync<OrganizationMember>
    {
        Task<List<OrganizationMember>> GetOrganizationMembersByOrganizationIdAsync(Guid organizationId);
    }
}