using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Core.Features.Organization.Queries.Result
{
    public class OrganizationResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Address { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public string? LogoUrl { get; set; }
        public OrganizationType Type { get; set; }
        // public ICollection<OrganizationMember> Members { get; set; } =
        //    new List<OrganizationMember>();
        // public ICollection<Farm> Farms { get; set; } = new List<Farm>(); // الأصول الثابتة
        // public ICollection<MarketItem> MarketItems { get; set; } = new List<MarketItem>(); // المنتجات المعروضة

    }
}