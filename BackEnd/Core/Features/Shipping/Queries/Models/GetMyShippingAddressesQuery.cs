using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using Core.Base;
using Core.Features.Shipping.Queries.Results;
using MediatR;

namespace Core.Features.Shipping.Queries.Models
{
    public class GetMyShippingAddressesQuery : IRequest<Response<List<ShippingAddressResponse>>>
    {
        [JsonIgnore]
        public Guid CurrentUserId { get; set; }
    }
}
