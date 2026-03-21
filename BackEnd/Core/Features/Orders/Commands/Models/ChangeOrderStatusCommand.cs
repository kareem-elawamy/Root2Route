using System;
using System.Text.Json.Serialization;
using Core.Base;
using MediatR;
using Domain.Enums;

namespace Core.Features.Orders.Commands.Models
{
    public class ChangeOrderStatusCommand : IRequest<Response<string>>
    {
        public Guid OrderId { get; set; }
        
        public OrderStatus NewStatus { get; set; }
        
        public decimal ShippingFees { get; set; }

        [JsonIgnore] 
        public Guid OrganizationId { get; set; } 
    }
}
