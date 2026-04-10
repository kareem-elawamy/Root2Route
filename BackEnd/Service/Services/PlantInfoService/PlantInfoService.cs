using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enums;
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
            var exists = await _plantInfoRepo.GetTableNoTracking()
                .AnyAsync(p => p.Name!.ToLower() == plantInfo.Name!.ToLower() && !p.IsDeleted);

            if (exists) return "Exists";

            if (imageFile != null)
            {
                var imageUrl = await _fileService.UploadImageAsync("plant-images", imageFile);
                plantInfo.ImageUrl = imageUrl;
            }

            await _plantInfoRepo.AddAsync(plantInfo);
            return "Success";
        }

        public async Task<string> DeletePlantInfoAsync(PlantInfo plantInfo)
        {
            try
            {
                plantInfo.IsDeleted = true;
                plantInfo.UpdatedAt = DateTime.UtcNow;
                await _plantInfoRepo.UpdateAsync(plantInfo);
                return "Success";
            }
            catch (Exception)
            {
                return "Failed";
            }
        }

        public async Task<string> EditPlantInfoAsync(PlantInfo requestPlant, IFormFile? imageFile = null)
        {
            var existingPlant = await _plantInfoRepo.GetByIdAsync(requestPlant.Id);

            if (existingPlant == null) return "NotFound";

            if (!string.IsNullOrEmpty(requestPlant.Name) && requestPlant.Name != existingPlant.Name)
            {
                var isNameTaken = await IsPlantInfoExistsAsync(requestPlant.Name);
                if (isNameTaken) return "Exists";
                existingPlant.Name = requestPlant.Name;
            }

            if (!string.IsNullOrEmpty(requestPlant.ScientificName)) existingPlant.ScientificName = requestPlant.ScientificName;
            if (!string.IsNullOrEmpty(requestPlant.Description)) existingPlant.Description = requestPlant.Description;
            if (!string.IsNullOrEmpty(requestPlant.IdealSoil)) existingPlant.IdealSoil = requestPlant.IdealSoil;
            if (!string.IsNullOrEmpty(requestPlant.MedicalBenefits)) existingPlant.MedicalBenefits = requestPlant.MedicalBenefits;
            if (!string.IsNullOrEmpty(requestPlant.PlantingSeason)) existingPlant.PlantingSeason = requestPlant.PlantingSeason;

            if (imageFile != null)
            {
                var newImageUrl = await _fileService.UploadImageAsync("plant-images", imageFile);


                // if (!string.IsNullOrEmpty(existingPlant.ImageUrl)) 
                //    _fileService.DeleteImage(existingPlant.ImageUrl); 
                existingPlant.ImageUrl = newImageUrl;
            }

            existingPlant.UpdatedAt = DateTime.UtcNow;

            await _plantInfoRepo.UpdateAsync(existingPlant);
            return "Success";
        }

        public IQueryable<PlantInfo> FilterPlantinfoPaginatedQuerable(PlantInfoOrderingEnum orderingEnum, string search)
        {
            var querable = _plantInfoRepo.GetTableNoTracking().AsQueryable();
            if (search != null)
            {
                querable = querable.Where(x => x.Name!.Contains(search) || x.PlantingSeason!.Contains(search) || x.ScientificName!.Contains(search));
            }
            switch (orderingEnum)
            {
                case PlantInfoOrderingEnum.Id:
                    querable = querable.OrderBy(x => x.Id);
                    break;
                case PlantInfoOrderingEnum.Name:
                    querable = querable.OrderBy(x => x.Name);
                    break;
                case PlantInfoOrderingEnum.ScientificName:
                    querable = querable.OrderBy(x => x.ScientificName);
                    break;
                case PlantInfoOrderingEnum.PlantingSeason:
                    querable = querable.OrderBy(x => x.PlantingSeason);
                    break;
            }

            return querable;
        }

        public async Task<List<PlantInfo>> GetAllPlantInfosAsync()
        {
            return await _plantInfoRepo.GetTableNoTracking()
                                       .Where(x => !x.IsDeleted)
                                       .OrderByDescending(x => x.CreatedAt) // تحسين: ترتيب البيانات الأحدث أولاً
                                       .ToListAsync();
        }

        public Task<PlantInfo?> GetPlantInfoByIdAsync(Guid id)
        {
            return _plantInfoRepo.GetTableNoTracking()
                                 .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        }

        public async Task<bool> IsPlantInfoExistsAsync(string name)
        {
            return await _plantInfoRepo.GetTableNoTracking()
                .AnyAsync(p => p.Name!.ToLower() == name.ToLower() && !p.IsDeleted);
        }

        public async Task<string> UploadPlantInfoImageAsync(Guid plantInfoId, IFormFile file)
        {
            var plantInfo = await _plantInfoRepo.GetByIdAsync(plantInfoId);
            if (plantInfo == null) return "NotFound";
            var imageUrl = await _fileService.UploadImageAsync("plant-images", file);
            plantInfo.ImageUrl = imageUrl;
            plantInfo.UpdatedAt = DateTime.UtcNow;
            await _plantInfoRepo.UpdateAsync(plantInfo);
            return "Success";

        }
    }
}