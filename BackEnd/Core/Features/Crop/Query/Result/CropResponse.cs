using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Features.Crop.Query.Result
{
    public class CropResponse
    {
        public Guid Id { get; set; }
        public string FarmName { get; set; } // نجيب الاسم من الـ Farm Navigation
        public string Status { get; set; }
        public double PlantedArea { get; set; }
        public DateTime PlantingDate { get; set; }



        public string PlantName { get; set; }
        public string ScientificName { get; set; }
        public string PlantImageUrl { get; set; }

    }
}