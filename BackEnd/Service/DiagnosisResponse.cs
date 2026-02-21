using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Service
{
    public class DiagnosisResponse
    {
        [JsonPropertyName("prediction")]
        public string Prediction { get; set; } = string.Empty;

        [JsonPropertyName("confidence")]
        public double Confidence { get; set; }

        [JsonPropertyName("leaf_detected")] // هذا هو مفتاح الحل
        public bool LeafDetected { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        // أضف هذا الحقل لتخزين نصيحة جيميني
        public string ExpertAdvice { get; set; } = string.Empty;
    }
}