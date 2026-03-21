using System;
using Core.Base;
using MediatR;

namespace Core.Features.Orders.Commands.Models
{
    public class CancelOrderCommand : IRequest<Response<string>>
    {
        public Guid OrderId { get; set; }
        public Guid BuyerId { get; set; } 
    }
}