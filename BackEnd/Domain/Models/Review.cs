using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Common;

namespace Domain.Models
{
    public class Review : BaseEntity
    {
        public Guid ReviewerId { get; set; }
        [ForeignKey(nameof(ReviewerId))]
        public ApplicationUser? Reviewer { get; set; }

        public Guid TargetOrganizationId { get; set; }
        [ForeignKey(nameof(TargetOrganizationId))]
        public Organization? TargetOrganization { get; set; }

        public Guid? ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product? Product { get; set; }

        public Guid OrderId { get; set; }
        [ForeignKey(nameof(OrderId))]
        public Order? Order { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        [MaxLength(500)]
        public string? Comment { get; set; }
    }
}
