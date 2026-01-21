using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Infrastructure.Base;

namespace Infrastructure.Abstract
{
    public interface IPlantInfoRepo : IGenericRepositoryAsync<PlantInfo>
    {
        Task<List<PlantInfo>> GetAllPlantInfosAsync();
    }
}