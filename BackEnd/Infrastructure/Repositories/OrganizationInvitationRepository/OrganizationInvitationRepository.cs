using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.OrganizationInvitationRepository
{
    public class OrganizationInvitationRepository : GenericRepositoryAsync<OrganizationInvitation>, IOrganizationInvitationRepository
    {
        public OrganizationInvitationRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}