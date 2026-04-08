using System;

namespace Service.DTOs.DashBoardDto
{
    public class OrgDashboardOverviewDto
    {
        public decimal TotalRevenue { get; set; }
        public int ActiveAuctions { get; set; }
        public int PendingOrders { get; set; }
        public int UnreadMessages { get; set; }
    }
}
