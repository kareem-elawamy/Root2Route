using Domain.Models;
using Infrastructure.Base;
using Infrastructure.Data;

namespace Infrastructure.Repositories.OrganizationDocumentRepository
{
    public class OrganizationDocumentRepository : GenericRepositoryAsync<OrganizationDocument>, IOrganizationDocumentRepository
    {
        public OrganizationDocumentRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
