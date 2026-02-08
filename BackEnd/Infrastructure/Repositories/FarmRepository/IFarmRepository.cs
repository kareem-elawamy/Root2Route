using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.FarmRepository
{
    public interface IFarmRepository: IGenericRepositoryAsync<Farm>
    {
        Task<List<Farm>> GetFarmsByOrganizationIdAsync(Guid organizationId);
    }
}