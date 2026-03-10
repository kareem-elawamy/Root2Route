using System;
using Core.Base;
using Core.Features.Orders.Queries.Results;
using MediatR;

namespace Core.Features.Orders.Queries.Models
{
    public class GetOrderByIdQuery : IRequest<Response<OrderResponse>>
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; } 

        public GetOrderByIdQuery(Guid orderId, Guid userId)
        {
            OrderId = orderId;
            UserId = userId;
        }
    }
}