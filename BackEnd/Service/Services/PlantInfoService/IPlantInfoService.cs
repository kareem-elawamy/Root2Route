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
        public Task<string> EditPlantInfoAsync(PlantInfo plantInfo, IFormFile? imageFile = null);
        public Task<bool> IsPlantInfoExistsAsync(string name);
        public Task<PlantInfo?> GetPlantInfoByIdAsync(Guid id);
        public Task<string> DeletePlantInfoAsync(PlantInfo plantInfo);


    }
}