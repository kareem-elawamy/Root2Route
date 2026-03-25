using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    public class Auction : BaseEntity
    {
        [Required, MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal StartPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal MinimumBidIncrement { get; set; } = 1.00m;

        [Column(TypeName = "decimal(18,2)")]
        public decimal? ReservePrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CurrentHighestBid { get; set; }

        public AuctionStatus Status { get; set; }

        public Guid ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product? Product { get; set; }
        public Guid? HighestBidderId { get; set; }
        public ApplicationUser? HighestBidder { get; set; }
        public Guid? OrderId { get; set; }
        public Order? Order { get; set; }
        public ICollection<Bid> Bids { get; set; } = new List<Bid>();
    }
}
