using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Organization : BaseEntity
    {
        [Required(ErrorMessage = "Organization Name is required")]
        [MaxLength(100)]
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Address { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public string? LogoUrl { get; set; }

        [Required] // ضيف دي
        public OrganizationType Type { get; set; }
        public Guid OwnerId { get; set; }
        public ApplicationUser? Owner { get; set; }
        public OrganizationStatus OrganizationStatus { get; set; }
        public ICollection<OrganizationMember> Members { get; set; } =
            new List<OrganizationMember>();
        // public ICollection<Farm> Farms { get; set; } = new List<Farm>(); // الأصول الثابتة
        public ICollection<MarketItem> MarketItems { get; set; } = new List<MarketItem>(); // المنتجات المعروضة
    }
}
