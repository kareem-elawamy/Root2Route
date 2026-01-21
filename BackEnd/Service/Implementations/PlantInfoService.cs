using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Infrastructure.Abstract;
using Service.Abstract;

namespace Service.Implementations
{
    public class PlantInfoService : IPlantInfoService
    {
        private readonly IPlantInfoRepo _plantInfoRepo;
        public PlantInfoService(IPlantInfoRepo plantInfoRepo)
        {
            _plantInfoRepo = plantInfoRepo;
        }
        public async Task<List<PlantInfo>> GetAllPlantInfosAsync()
        {
            return await _plantInfoRepo.GetAllPlantInfosAsync();
        }
        
    }
}