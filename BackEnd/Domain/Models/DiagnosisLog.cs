using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    public class DiagnosisLog : BaseEntity
    {
        public Guid UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser? User { get; set; }

        public string Prediction { get; set; } = string.Empty;
        public double Confidence { get; set; }
        
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? City { get; set; }
        public string? Region { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
