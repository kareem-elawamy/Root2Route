using System;
using System.Collections.Generic;
using Core.Base;
using MediatR;

namespace Core.Features.Orders.Commands.Models
{
    public class OrderItemRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class CreateOrderCommand : IRequest<Response<string>>
    {
        public Guid BuyerId { get; set; }

        public string ReceiverName { get; set; } = null!;
        public string ReceiverPhone { get; set; } = null!;
        public string ShippingCity { get; set; } = null!;
        public string ShippingStreet { get; set; } = null!;
        public string? BuildingNumber { get; set; }

        public List<OrderItemRequest> Items { get; set; } = new List<OrderItemRequest>();
    }
}