using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class OrderItem : BaseEntity
    {
        public Guid OrderId { get; set; }
        public Order? Order { get; set; }

        public Guid MarketItemId { get; set; }
        public MarketItem? MarketItem { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}