using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class MarketItem : BaseEntity
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }
        public int? StockQuantity { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsForAuction { get; set; } // هل معروض للمزاد؟
        public bool IsForDirectSale { get; set; } // هل معروض للبيع المباشر؟
        public Guid OrganizationId { get; set; }
        public Organization? Organization { get; set; }
    }
}