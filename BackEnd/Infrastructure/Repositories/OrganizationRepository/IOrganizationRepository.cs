using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories.OrganizationRepository
{
    public interface IOrganizationRepository : IGenericRepositoryAsync<Organization>
    {
        Task<List<Organization>> GetAllOrganizationsByOwnerId(Guid ownerId);
    }
}
