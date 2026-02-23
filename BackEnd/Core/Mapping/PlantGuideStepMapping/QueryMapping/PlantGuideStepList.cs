using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Mapping.PlantGuideStepMapping
{
    public partial class PlantGuideStepProfile
    {
        public void ConfigurePlantGuideStepListMapping()
        {
            CreateMap<Domain.Models.PlantGuideStep, Core.Features.PlantGuideStep.Queries.Result.PlantGuideStepListResponse>();
        }

    }
}