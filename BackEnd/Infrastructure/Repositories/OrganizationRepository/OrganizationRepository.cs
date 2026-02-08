using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.OrganizationRepository
{
    public class OrganizationRepository : GenericRepositoryAsync<Organization> , IOrganizationRepository
    {
        public OrganizationRepository(ApplicationDbContext context) : base(context)
        {
            
        }

        public Task<List<Organization>> GetAllOrganizationsByOwnerId(Guid ownerId)
        {
            var organizations = _dbContext.Organizations.Where(o=> o.OwnerId == ownerId).ToListAsync();
            return organizations ;
        }
    }
}
