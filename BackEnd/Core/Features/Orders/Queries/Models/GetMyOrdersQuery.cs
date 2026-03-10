using System;
using System.Collections.Generic;
using Core.Base;
using Core.Features.Orders.Queries.Results;
using MediatR;

namespace Core.Features.Orders.Queries.Models
{
    public class GetMyOrdersQuery : IRequest<Response<List<OrderResponse>>>
    {
        public Guid BuyerId { get; set; }
        public GetMyOrdersQuery(Guid buyerId) { BuyerId = buyerId; }
    }
}