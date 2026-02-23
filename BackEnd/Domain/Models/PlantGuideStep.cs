namespace Domain.Models
{
    public class PlantGuideStep : BaseEntity
    {
        public int StepOrder { get; set; } // ترتيب الخطوة (1، 2، 3)

        [Required]
        public string? Title { get; set; }

        [Required]
        public string? Instruction { get; set; } // تفاصيل: "قم بالري كل يومين"
        public Guid PlantInfoId { get; set; } // FK to PlantInfo
        public PlantInfo? PlantInfo { get; set; } // Navigation property
    }
}
