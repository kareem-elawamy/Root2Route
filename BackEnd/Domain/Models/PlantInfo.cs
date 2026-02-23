namespace Domain.Models
{
    public class PlantInfo : BaseEntity
    {
        [Required]
        public string? Name { get; set; } // مثلاً: نعناع
        public string ScientificName { get; set; } = string.Empty; // الاسم العلمي
        public string Description { get; set; } = string.Empty; // وصف النبات

        [Required]
        public string? IdealSoil { get; set; } // التربة المثالية
        public string MedicalBenefits { get; set; } = string.Empty; // للأعشاب الطبية

        public string? PlantingSeason { get; set; } // موسم الزراعة
        public string? ImageUrl { get; set; } // رابط صورة للنبات

        // خطوات الزراعة (One-to-Many)
        public ICollection<PlantGuideStep>? GuideSteps { get; set; }
    }
}
