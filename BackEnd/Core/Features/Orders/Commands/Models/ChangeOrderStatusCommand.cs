using System;
using Core.Base;
using Domain.Enums;
using MediatR;

namespace Core.Features.Orders.Commands.Models
{
    public class ChangeOrderStatusCommand : IRequest<Response<string>>
    {
        public Guid OrderId { get; set; }
        public OrderStatus NewStatus { get; set; }
        public Guid OrganizationId { get; set; }
    }
}