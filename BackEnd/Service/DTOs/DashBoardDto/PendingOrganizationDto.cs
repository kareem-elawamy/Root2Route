using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.DTOs.DashBoardDto
{
    public class PendingOrganizationDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; }
    }
}