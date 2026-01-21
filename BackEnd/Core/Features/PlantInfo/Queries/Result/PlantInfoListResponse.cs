using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Features.PlantInfo.Queries.Result
{
    public class PlantInfoListResponse
    {
        public Guid Id { get; set; } // معرف فريد للنبات
        public string? Name { get; set; } // مثلاً: نعناع
        public string ScientificName { get; set; } = string.Empty; // الاسم العلمي
        public string Description { get; set; } = string.Empty; // وصف النبات
        public string? IdealSoil { get; set; } // التربة المثالية
        public string MedicalBenefits { get; set; } = string.Empty; // للأعشاب
        public string? PlantingSeason { get; set; } // موسم الزراعة

    }
}