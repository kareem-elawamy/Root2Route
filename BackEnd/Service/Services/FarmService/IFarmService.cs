using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enums;

namespace Service.Services.FarmService
{
    public interface IFarmService
    {
        public Task<List<Farm>> GetFarmsAsync();
        public Task<FarmServiceResult> AddFarm(Farm farm);
        public Task<string> EditFarm(Farm farm);
        public Task<string> DeleteFarm(Farm farm);
        public Task<List<Farm>> GetFarmsByOrganizationIdAsync(Guid organizationId);

        
        
    }
}