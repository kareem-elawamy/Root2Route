using System;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Common;
using Domain.Enums;

namespace Domain.Models
{
    public class Shipment : BaseEntity
    {
        public Guid OrderId { get; set; }
        [ForeignKey(nameof(OrderId))]
        public Order? Order { get; set; }

        public string TrackingNumber { get; set; } = string.Empty;
        public string CarrierName { get; set; } = string.Empty;
        public string? DriverPhone { get; set; }
        public ShipmentStatus Status { get; set; } = ShipmentStatus.Pending;
    }
}
