

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.PlantGuideStepRepository
{
    public interface IPlantGuideStepRepository : IGenericRepositoryAsync<PlantGuideStep>
    {
        Task<List<PlantGuideStep>> GetAllPlantGuideStepsAsync();
        Task<PlantGuideStep> GetPlantGuideStepByIdAsync(Guid id);
        Task<List<PlantGuideStep>> GetPlantGuideStepAsyncByPlantId(Guid plantId);
        Task<List<PlantGuideStep>> GetPlantGuideStepAsyncByName(string name);

    }
}