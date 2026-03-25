using System;
using System.Text.Json.Serialization;
using Core.Base;
using MediatR;

namespace Core.Features.Shipping.Commands.Models
{
    public class AddShippingAddressCommand : IRequest<Response<Guid>>
    {
        [JsonIgnore]
        public Guid CurrentUserId { get; set; }
        public string Label { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public bool IsDefault { get; set; } = false;
    }
}
