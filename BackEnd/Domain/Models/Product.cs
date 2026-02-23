using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Product : MarketItem
    {
        public string? Barcode { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public WeightUnit? WeightUnit { get; set; }

        // التتبع (Traceability)
        public Guid? SourceCropId { get; set; }

        // هل المنتج ده مادة خام (محصول) ولا منتج مصنع (مبيد/سماد)؟
        public ProductType ProductType { get; set; }
    }
}
