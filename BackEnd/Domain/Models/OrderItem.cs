using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    public class OrderItem : BaseEntity
    {
        public Guid OrderId { get; set; }
        public Order? Order { get; set; }

        public Guid ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product? Product { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
