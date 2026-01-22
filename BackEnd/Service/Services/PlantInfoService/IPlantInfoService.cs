using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.AspNetCore.Http;

namespace Service.Services.PlantInfoService
{
    public interface IPlantInfoService
    {
        public Task<List<PlantInfo>> GetAllPlantInfosAsync();
        public Task<string> CreatePlantInfoAsync(PlantInfo plantInfo, IFormFile? imageFile = null);
    }
}