using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories.OrganizationRepository
{
    public class OrganizationRepository : GenericRepositoryAsync<Organization> , IOrganizationRepository
    {
        public OrganizationRepository(ApplicationDbContext context) : base(context)
        {
            
        }
    }
}
