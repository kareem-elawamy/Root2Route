using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Crop : MarketItem
    {

        public string? BatchNumber { get; set; } // رقم التشغيلة (للتتبع)
        public CropStatus Status { get; set; }
        public int AvailableQuantity { get; set; } // الكمية المتاحة بالكيلو/الطن
        public Guid PlantInfoId { get; set; }
        public PlantInfo? PlantInfo { get; set; }
        public Guid OrganizationId { get; set; }
        public Organization? Organization { get; set; }
        public ICollection<CropActivityLog>? Activities { get; set; }

    }
}