namespace Core.Features.Product.Queries.Results
{
    public class ProductResponse
    {
        public Guid Id { get; set; }
        public Guid OrganizationId { get; set; }

        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public List<string> Images { get; set; } = new List<string>();
        public string? MainImageUrl { get; set; }
        public int StockQuantity { get; set; }

        public bool IsAvailableForDirectSale { get; set; }
        public decimal DirectSalePrice { get; set; }

        public bool IsAvailableForAuction { get; set; }
        public decimal StartBiddingPrice { get; set; }

        public string? Barcode { get; set; }
        public DateTime? ExpiryDate { get; set; }

        public string? WeightUnit { get; set; }
        public string? ProductType { get; set; }
    }
}