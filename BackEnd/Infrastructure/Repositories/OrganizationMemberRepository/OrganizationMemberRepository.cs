using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.OrganizationMemberRepository
{
    public class OrganizationMemberRepository : GenericRepositoryAsync<OrganizationMember>, IOrganizationMemberRepository
    {
        public OrganizationMemberRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
        public async Task<List<OrganizationMember>> GetOrganizationMembersByOrganizationIdAsync(Guid organizationId)
        {
            return await GetTableAsTracking().Where(x => x.OrganizationId == organizationId).ToListAsync();
        }
    }
}