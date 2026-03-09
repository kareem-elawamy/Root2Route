using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    public class ProductImage : BaseEntity
    {
        [Required]
        public string ImageUrl { get; set; } = null!;

        // ميزة إضافية: عشان لو عايز تخلي صورة معينة هي الـ Cover أو الصورة الرئيسية للمنتج
        public bool IsMain { get; set; }

        // ربط الصورة بالمنتج
        public Guid ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product? Product { get; set; }
    }
}