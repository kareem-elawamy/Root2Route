using Domain.Models;
using Domain.Enums;
using Infrastructure.Repositories.CropRepository;
using Infrastructure.Repositories.FarmRepository;
using Microsoft.EntityFrameworkCore; // مهم عشان الـ Include
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Services.CropService
{
    public class CropService : ICropService
    {
        private readonly ICropRepository _cropRepository;
        private readonly IFarmRepository _farmRepository;

        public CropService(ICropRepository cropRepository, IFarmRepository farmRepository)
        {
            _cropRepository = cropRepository;
            _farmRepository = farmRepository;
        }

        // ===========================
        // 1. القراءة (Queries)
        // ===========================
        // ملاحظة: الـ Global Query Filter في الـ DbContext سيتكفل بإخفاء المحذوف تلقائياً

        public async Task<List<Crop>> GetAllCropsAsync()
        {
            return await _cropRepository.GetTableNoTracking()
                                        .Include(c => c.PlantInfo)
                                        .Include(c => c.Farm)
                                        .ToListAsync();
        }

        public async Task<Crop?> GetCropByIdAsync(Guid id)
        {
            var crop = await _cropRepository.GetTableNoTracking()
                                            .Include(c => c.Farm)
                                            .Include(c => c.PlantInfo)
                                            .Include(c => c.Activities) // لو عايز تعرض السجلات كمان
                                            .FirstOrDefaultAsync(c => c.Id == id);
            return crop;
        }

        public async Task<List<Crop>> GetCropsByFarmIdAsync(Guid farmId)
        {
            return await _cropRepository.GetTableNoTracking()
                                        .Where(x => x.FarmId == farmId)
                                        .Include(c => c.PlantInfo)
                                        .ToListAsync();
        }

        // ===========================
        // 2. الكتابة (Commands)
        // ===========================

        public async Task<CropServiceResult> AddCropAsync(Crop crop)
        {
            // التحقق من وجود المزرعة (وأنها غير محذوفة أيضاً بفضل الفلتر)
            var farmExists = await _farmRepository.GetByIdAsync(crop.FarmId);
            if (farmExists == null) return CropServiceResult.FarmNotFound;

            // التحقق من منطقية التواريخ
            if (crop.ExpectedHarvestDate < crop.PlantingDate)
            {
                return CropServiceResult.InvalidDates;
            }

            crop.Status = CropStatus.Seeds;

            // التأكد من تصفير قيم الحذف عند الإنشاء (كإجراء احترازي)
            crop.IsDeleted = false;

            await _cropRepository.AddAsync(crop);
            return CropServiceResult.Success;
        }

        public async Task<CropServiceResult> UpdateCropAsync(Crop crop)
        {
            var existingCrop = await _cropRepository.GetByIdAsync(crop.Id);
            if (existingCrop == null) return CropServiceResult.NotFound;

            // تحديث البيانات
            existingCrop.PlantedArea = crop.PlantedArea;
            existingCrop.ExpectedHarvestDate = crop.ExpectedHarvestDate;
            existingCrop.Status = crop.Status;
            existingCrop.YieldUnit = crop.YieldUnit;
            existingCrop.PlantInfoId = crop.PlantInfoId;

            if (existingCrop.ExpectedHarvestDate < existingCrop.PlantingDate)
            {

                return CropServiceResult.InvalidDates;
            }

            await _cropRepository.UpdateAsync(existingCrop);
            return CropServiceResult.Success;
        }

        // ⭐ التعديل الجوهري هنا (Soft Delete)
        public async Task<CropServiceResult> DeleteCropAsync(Guid id)
        {
            var crop = await _cropRepository.GetByIdAsync(id);
            if (crop == null) return CropServiceResult.NotFound;

            // بدلاً من الحذف الفعلي، نقوم بالتعديل
            crop.IsDeleted = true;
            crop.UpdatedAt = DateTime.UtcNow;

            // نستخدم UpdateAsync بدلاً من DeleteAsync
            await _cropRepository.UpdateAsync(crop);

            return CropServiceResult.Success;
        }

        public async Task<CropServiceResult> RegisterHarvestAsync(Guid cropId, double actualYield)
        {
            var crop = await _cropRepository.GetByIdAsync(cropId);
            if (crop == null) return CropServiceResult.NotFound;

            crop.Status = CropStatus.Harvested;
            crop.ActualYieldQuantity = actualYield;

            await _cropRepository.UpdateAsync(crop);
            return CropServiceResult.Success;
        }
        public async Task<Crop?> GetCropWithDetailsAsync(Guid id)
        {
            return await _cropRepository.GetTableNoTracking()
                .Include(x => x.PlantInfo)
                .Include(x => x.Farm)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        }
    }
}