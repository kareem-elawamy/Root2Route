using System;
using System.Text.Json.Serialization;
using Core.Base;
using MediatR;

namespace Core.Features.Shipping.Commands.Models
{
    public class DispatchOrderCommand : IRequest<Response<string>>
    {
        public Guid OrderId { get; set; }
        public string CarrierName { get; set; } = string.Empty;
        public string TrackingNumber { get; set; } = string.Empty;
        public string? DriverPhone { get; set; }

        [JsonIgnore]
        public Guid CurrentUserId { get; set; }
    }
}
