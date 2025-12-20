using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Product : MarketItem
    {
        public string? Barcode { get; set; }
        public DateTime? ExpiryDate { get; set; } // تاريخ الصلاحية
        public WeightUnit? WeightUnit { get; set; } // كجم، لتر، عبوة

    }
}