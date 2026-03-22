using System;
using System.Collections.Generic;

namespace Core.Features.Orders.Queries.Results
{
    public class OrderItemResponse
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice => Quantity * UnitPrice;
    }

    public class OrderStatusHistoryResponse
    {
        public string OldStatus { get; set; } = null!;
        public string NewStatus { get; set; } = null!;
        public DateTime ChangedAt { get; set; }
        public string? Note { get; set; }
    }

    public class PaymentResponse
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public string PaymentStatus { get; set; } = null!;
        public DateTime? PaidAt { get; set; }
    }

    public class OrderResponse
    {
        public Guid Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = null!;
        public Guid BuyerId { get; set; }
        public Guid OrganizationId { get; set; }
        public string? OrganizationName { get; set; }

        public string? ReceiverName { get; set; }
        public string? ReceiverPhone { get; set; }
        public string? ShippingCity { get; set; }
        public string? ShippingStreet { get; set; }
        public string? BuildingNumber { get; set; }
        public string PaymentMethod { get; set; } = "CashOnDelivery"; 
        public string PaymentStatus { get; set; } = "Pending";
        public decimal ShippingFees { get; set; }
        public decimal FinalTotal => TotalAmount + ShippingFees;

        public List<OrderItemResponse> Items { get; set; } = new();
        public List<OrderStatusHistoryResponse> StatusHistory { get; set; } = new();
        public PaymentResponse? Payment { get; set; }
    }
}
