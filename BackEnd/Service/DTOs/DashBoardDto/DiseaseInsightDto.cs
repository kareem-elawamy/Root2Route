using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.DTOs.DashBoardDto
{
    public class DiseaseInsightDto
    {
        public string DiseaseName { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}