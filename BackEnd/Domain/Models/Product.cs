using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;

namespace Domain.Models
{
    // شيلنا الوراثة من MarketItem وبقت من BaseEntity مباشرة
    public class Product : BaseEntity
    {
        [Required, MaxLength(150)]
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int StockQuantity { get; set; }

        // --- إعدادات البيع المباشر ---
        public bool IsAvailableForDirectSale { get; set; } = true;

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue)]
        public decimal DirectSalePrice { get; set; }

        // --- إعدادات المزاد ---
        public bool IsAvailableForAuction { get; set; } = false;

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue)]
        public decimal StartBiddingPrice { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; } = null!; // لحماية المزايدات (Concurrency)

        // --- خصائص المنتج الإضافية ---
        public string? Barcode { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public WeightUnit? WeightUnit { get; set; }
        public ProductType ProductType { get; set; }

        // --- العلاقات (Relations) ---

        // المالك (المؤسسة أو المتجر)
        public Guid OrganizationId { get; set; }
        [ForeignKey(nameof(OrganizationId))]
        public Organization? Organization { get; set; }

        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
        public ProductStatus Status { get; set; } = ProductStatus.Pending; // الحالة الافتراضية
        public string? RejectionReason { get; set; } // سبب الرفض لو الأدمن رفضه

    }
}