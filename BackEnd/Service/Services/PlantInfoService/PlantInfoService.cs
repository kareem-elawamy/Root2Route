using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Infrastructure.Repositories.PlantInfoRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Service.Services.FileService;

namespace Service.Services.PlantInfoService
{
    public class PlantInfoService : IPlantInfoService
    {
        private readonly IPlantInfoRepository _plantInfoRepo;
        private readonly IFileService _fileService;
        public PlantInfoService(IFileService fileService, IPlantInfoRepository plantInfoRepo)
        {
            _fileService = fileService;
            _plantInfoRepo = plantInfoRepo;
        }

        public async Task<string> CreatePlantInfoAsync(PlantInfo plantInfo, IFormFile? imageFile = null)
        {
            if (imageFile != null)
            {
                var imageUrl = await _fileService.UploadImageAsync("plant-images", imageFile);
                plantInfo.ImageUrl = imageUrl;
            }

            var exists = await _plantInfoRepo.GetTableNoTracking()
                .AnyAsync(p => p.Name!.ToLower() == plantInfo.Name!.ToLower());

            if (exists)
            {
                return "Exists";
            }

            await _plantInfoRepo.AddAsync(plantInfo);
            return "Success";
        }

        public async Task<List<PlantInfo>> GetAllPlantInfosAsync()
        {
            return await _plantInfoRepo.GetAllPlantInfosAsync();
        }

    }
}