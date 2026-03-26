using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    public class Order : BaseEntity
    {
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }

        public Guid BuyerId { get; set; }
        [ForeignKey(nameof(BuyerId))]
        public ApplicationUser? Buyer { get; set; }

        public Guid OrganizationId { get; set; }
        [ForeignKey(nameof(OrganizationId))]
        public Organization? Organization { get; set; }

        public OrderStatus Status { get; set; }
        public string? ReceiverName { get; set; }
        public string? ReceiverPhone { get; set; }
        public string? ShippingCity { get; set; }
        public string? ShippingStreet { get; set; }
        public string? BuildingNumber { get; set; }
        public decimal ShippingFees { get; set; } = 0;
        public decimal PlatformFee { get; set; } = 0;
        public decimal FinalTotal => TotalAmount + ShippingFees;
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.CashOnDelivery;
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
        public ICollection<OrderItem>? OrderItems { get; set; }
    }
}
