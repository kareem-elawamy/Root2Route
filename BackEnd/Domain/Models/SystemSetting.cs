using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    public class SystemSetting : BaseEntity
    {
        [Column(TypeName = "decimal(18,2)")]
        public decimal PlatformFeePercentage { get; set; } = 5.0m;

        [Column(TypeName = "decimal(18,2)")]
        public decimal StandardShippingFee { get; set; } = 50.0m;

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}
