using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Features.ModelAI.Query.Result
{
    public class AnalyzePlantImageResponse
    {
        public string Prediction { get; set; } = string.Empty;
        public double Confidence { get; set; }
        public bool LeafDetected { get; set; }
        public string ExpertAdvice { get; set; } = string.Empty; // أضف هذا السطر
        public string Status { get; set; } = string.Empty;

    }
}