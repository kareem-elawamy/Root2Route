using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class CropActivityLog : BaseEntity
    {
        [Required, MaxLength(100)]
        ////
        public FarmActivityType? ActivityType { get; set; } // ري، تسميد، رش مبيدات

        [MaxLength(500)]
        public string? Description { get; set; }
        public DateTime ActivityDate { get; set; }
        public Guid CropId { get; set; }
        public Crop? Crop { get; set; }
        public Guid PerformedById { get; set; } // الموظف اللي عمل النشاط
        public ApplicationUser? PerformedBy { get; set; }
    }
}
