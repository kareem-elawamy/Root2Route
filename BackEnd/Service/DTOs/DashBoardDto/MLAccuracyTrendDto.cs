using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.DTOs.DashBoardDto
{
    public class MLAccuracyTrendDto
    {
        public string DateLabel { get; set; } = string.Empty;
        public double AverageConfidence { get; set; }
    }
}