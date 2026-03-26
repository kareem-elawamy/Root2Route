using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.DTOs
{
    public class DashboardOverviewDto
    {
        public decimal GrossRevenue { get; set; }
        public decimal PlatformFees { get; set; }
        public int PendingOrganizations { get; set; }
        public int TotalMLDiagnoses { get; set; }
    }
}