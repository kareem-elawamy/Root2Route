using System;

namespace Core.Features.Auctions.Queries.Results
{
    public class AuctionResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal StartPrice { get; set; }
        public decimal MinimumBidIncrement { get; set; }
        public decimal? ReservePrice { get; set; }
        public decimal CurrentHighestBid { get; set; }
        public string Status { get; set; } = string.Empty;
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public Guid? HighestBidderId { get; set; }
        public string? HighestBidderName { get; set; }
    }

    public class BidResponse
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime BidTime { get; set; }
        public Guid BidderId { get; set; }
        public string? BidderName { get; set; }
    }
}
