using System;
using System.Text.Json.Serialization;
using Core.Base;
using Core.Features.Orders.Queries.Results;
using MediatR;
using System.Collections.Generic;

namespace Core.Features.Orders.Queries.Models
{
    public class GetMyOrdersQuery : IRequest<Response<List<OrderResponse>>>
    {
        [JsonIgnore]
        public Guid CurrentUserId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
