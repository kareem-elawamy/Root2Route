using System;
using Domain.Enums;

namespace Service.DTOs.DashBoardDto
{
    /// <summary>
    /// Represents one row in the "Latest Orders" table on the overview page.
    /// </summary>
    public class LatestOrderDto
    {
        public Guid OrderId { get; set; }
        public string BuyerName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }

        public decimal ShippingFees { get; set; }

        public OrderStatus Status { get; set; }

        public DateTime OrderDate { get; set; }

        public string? ShippingCity { get; set; }
    }
}
