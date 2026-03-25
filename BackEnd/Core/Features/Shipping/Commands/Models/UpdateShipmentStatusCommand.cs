using System;
using System.Text.Json.Serialization;
using Core.Base;
using Domain.Enums;
using MediatR;

namespace Core.Features.Shipping.Commands.Models
{
    public class UpdateShipmentStatusCommand : IRequest<Response<string>>
    {
        public Guid ShipmentId { get; set; }
        public ShipmentStatus NewStatus { get; set; }

        [JsonIgnore]
        public Guid CurrentUserId { get; set; }
    }
}
