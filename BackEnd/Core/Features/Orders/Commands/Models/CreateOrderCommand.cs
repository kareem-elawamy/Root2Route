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

        public List<OrderItemRequest> Items { get; set; } = new List<OrderItemRequest>();
    }
}