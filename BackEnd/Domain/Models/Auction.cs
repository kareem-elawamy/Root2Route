using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    public class Auction : BaseEntity
    {
        [Required, MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        // التواريخ دي بتتحسب لما اليوزر يختار "مدة المزاد"
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; } // هنا بتخزن الوقت اللي هيخلص فيه

        [Column(TypeName = "decimal(18,2)")]
        public decimal StartPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CurrentHighestBid { get; set; }

        public AuctionStatus Status { get; set; }

        // ربط المزاد بالمنتج
        public Guid MarketItemId { get; set; }
        public MarketItem? MarketItem { get; set; }
        public Guid? HighestBidderId { get; set; }
        public ApplicationUser? HighestBidder { get; set; }
        public ICollection<Bid> Bids { get; set; } = new List<Bid>();
    }
}
