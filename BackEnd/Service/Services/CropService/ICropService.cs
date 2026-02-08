using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enums;

namespace Service.Services.CropService
{
    public interface ICropService
    {
        // استعلام (Queries)
        Task<List<Crop>> GetAllCropsAsync();
        Task<Crop?> GetCropByIdAsync(Guid id);
        Task<List<Crop>> GetCropsByFarmIdAsync(Guid farmId);

        // أوامر (Commands)
        Task<CropServiceResult> AddCropAsync(Crop crop);
        Task<CropServiceResult> UpdateCropAsync(Crop crop);
        Task<CropServiceResult> DeleteCropAsync(Guid id);

        // عملية خاصة: تسجيل الحصاد
        Task<CropServiceResult> RegisterHarvestAsync(Guid cropId, double actualYield);

        Task<Crop?> GetCropWithDetailsAsync(Guid id);
    }
}