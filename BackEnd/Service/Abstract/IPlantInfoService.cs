using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;

namespace Service.Abstract
{
    public interface IPlantInfoService
    {
        public Task<List<PlantInfo>> GetAllPlantInfosAsync();
    }
}