using System;
using Core.Base;
using Core.Features.Orders.Queries.Results;
using Domain.Enums;
using MediatR;

namespace Core.Features.Orders.Queries.Models
{
    public class GetReceivedOrdersQuery : IRequest<PaginatedResult<OrderResponse>>
    {
        public Guid OrganizationId { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public OrderStatus? Status { get; set; }
    }
}