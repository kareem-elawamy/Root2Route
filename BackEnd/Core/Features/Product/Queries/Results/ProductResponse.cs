namespace Core.Features.Product.Queries.Results
{
    public class ProductResponse
    {
        public Guid Id { get; set; }
        public Guid OrganizationId { get; set; }

        // بيانات البيع
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public int StockQuantity { get; set; }

        public bool IsAvailableForDirectSale { get; set; }
        public decimal DirectSalePrice { get; set; }

        public bool IsAvailableForAuction { get; set; }
        public decimal StartBiddingPrice { get; set; }

        // تفاصيل المنتج
        public string? Barcode { get; set; }
        public DateTime? ExpiryDate { get; set; }

        // هنرجعهم كـ String عشان الـ Frontend يفهمهم أسهل من الـ Enum (الأرقام)
        public string? WeightUnit { get; set; }
        public string? ProductType { get; set; }
    }
}