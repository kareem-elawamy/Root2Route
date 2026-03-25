using System;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Common;

namespace Domain.Models
{
    public class ShippingAddress : BaseEntity
    {
        public Guid UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public ApplicationUser? User { get; set; }

        public string Label { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public bool IsDefault { get; set; } = false;
    }
}
