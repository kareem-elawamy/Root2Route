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

    public class OrderResponse
    {
        public Guid Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = null!;
        public Guid BuyerId { get; set; }

        public string ReceiverName { get; set; } = null!;
        public string ReceiverPhone { get; set; } = null!;
        public string ShippingCity { get; set; } = null!;
        public string ShippingStreet { get; set; } = null!;
        public string? BuildingNumber { get; set; }
        public string PaymentMethod { get; set; } = "CashOnDelivery"; 
        public string PaymentStatus { get; set; } = "Pending";
        public List<OrderItemResponse> Items { get; set; } = new List<OrderItemResponse>();
        public decimal ShippingFees { get; set; }
        public decimal FinalTotal => TotalAmount + ShippingFees;
    }
}