using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace Core.Mapping.PlantGuideStepMapping
{
    public partial class PlantGuideStepProfile : Profile
    {
        public PlantGuideStepProfile()
        {
            ConfigurePlantGuideStepListMapping();
            ConfigurePlantGuideStepMapping();
        }
    }
}