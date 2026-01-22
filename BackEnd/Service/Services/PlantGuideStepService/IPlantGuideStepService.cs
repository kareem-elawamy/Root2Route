using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Services.PlantGuideStepService
{
    public interface IPlantGuideStepService
    {
        public Task<List<PlantGuideStep>> GetAllPlantGuideStepsAsync();
        public Task<PlantGuideStep> GetPlantGuideStepByIdAsync(Guid id);
        public Task<List<PlantGuideStep>> GetPlantGuideStepAsyncByPlantId(Guid plantId);
        public Task<List<PlantGuideStep>> GetPlantGuideStepAsyncByPlantName(string name);
    }
}