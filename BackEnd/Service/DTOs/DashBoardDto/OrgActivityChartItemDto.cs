using System;

namespace Service.DTOs.DashBoardDto
{
    /// <summary>
    /// One data-point on the "Activity Over Time" chart.
    /// Covers both net revenue and auction volume per time-bucket.
    /// </summary>
    public class OrgActivityChartItemDto
    {
        public string Month { get; set; } = string.Empty;
        public decimal NetRevenue { get; set; }
        public int AuctionVolume { get; set; }
    }
}
