using System;
using System.Collections.Generic;

namespace Core.Features.Shipping.Queries.Results
{
    public class ShippingAddressResponse
    {
        public Guid Id { get; set; }
        public string Label { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Street { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public bool IsDefault { get; set; }
    }
}
