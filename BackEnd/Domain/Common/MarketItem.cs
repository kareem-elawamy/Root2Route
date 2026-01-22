using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class MarketItem : BaseEntity
    {
        [Required, MaxLength(150)]
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public int StockQuantity { get; set; }

        // --- إعدادات البيع المباشر ---
        public bool IsAvailableForDirectSale { get; set; } = true;

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue)]
        public decimal DirectSalePrice { get; set; } // سعر "اشتري الآن"

        // --- إعدادات المزاد ---
        public bool IsAvailableForAuction { get; set; } = false;

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue)]
        public decimal StartBiddingPrice { get; set; } // سعر فتح المزاد



        // المالك (مين اللي بيبيع؟)
        public Guid OrganizationId { get; set; }
        [ForeignKey(nameof(OrganizationId))] // تأكد من وجود هذا
        public Organization? Organization { get; set; }
    }
}
