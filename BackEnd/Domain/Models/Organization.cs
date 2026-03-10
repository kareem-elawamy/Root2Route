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
        public ICollection<Product> products { get; set; } = new List<Product>(); // المنتجات المعروضة
    }
}
