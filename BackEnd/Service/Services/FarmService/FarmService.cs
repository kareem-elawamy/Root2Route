using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enums;
using Infrastructure.Repositories.FarmRepository;
using Infrastructure.Repositories.OrganizationRepository;

namespace Service.Services.FarmService
{
    public class FarmService : IFarmService
    {

        private readonly IFarmRepository _farmRepository;
        private readonly IOrganizationRepository _organizationRepository;

        public FarmService(IFarmRepository farmRepository, IOrganizationRepository organizationRepository)
        {
            _farmRepository = farmRepository;
            _organizationRepository = organizationRepository;
        }
        public async Task<FarmServiceResult> AddFarm(Farm farm)
        {
            var organization = await _organizationRepository.GetByIdAsync(farm.OrganizationId);

            if (organization == null)
                return FarmServiceResult.OrganizationNotFound; // ✅ Enum

            if (organization.Type == OrganizationType.Farm)
            {
                await _farmRepository.AddAsync(farm);
                return FarmServiceResult.Success; // ✅ Enum
            }

            return FarmServiceResult.OrganizationIsNotFarm; // ✅ Enum
        }
        public async Task<string> DeleteFarm(Farm farm)
        {
            try
            {
                farm.IsDeleted = true;
                farm.UpdatedAt = DateTime.UtcNow;
                await _farmRepository.UpdateAsync(farm);
                return "Success";
            }
            catch (Exception)
            {
                return "Failed";
            }
        }

        public Task<string> EditFarm(Farm farm)
        {
            throw new NotImplementedException();
        }

        public Task<List<Farm>> GetFarmsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<Farm>> GetFarmsByOrganizationIdAsync(Guid organizationId)
        {
            throw new NotImplementedException();
        }
    }
}