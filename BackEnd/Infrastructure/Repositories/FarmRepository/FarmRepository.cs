using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.FarmRepository
{
    public class FarmRepository : GenericRepositoryAsync<Farm>, IFarmRepository
    {
        public FarmRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        // Get Organization Farms
        public async Task<List<Farm>> GetFarmsByOrganizationIdAsync(Guid organizationId)
        {
            var farms = await _dbContext.Farms.Where(x => x.OrganizationId == organizationId)
                                            .AsNoTracking()
                                            .ToListAsync();
            return farms;
        }

    }
}