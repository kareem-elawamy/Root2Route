using System;

namespace Service.DTOs.DashBoardDto
{
    public class LiveBidActivityDto
    {
        public Guid BidId { get; set; }

        public string AuctionTitle { get; set; } = string.Empty;
        public string BidderName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime BidTime { get; set; }
    }
}
