using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Features.PlantGuideStep.Queries.Result;
using Domain.Models;

namespace Core.Mapping.PlantGuideStepMapping
{
    public partial class PlantGuideStepProfile
    {
        public void ConfigurePlantGuideStepMapping()
        {
            CreateMap<PlantGuideStep, PlantGuideStepListResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.StepOrder, opt => opt.MapFrom(src => src.StepOrder))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Instruction, opt => opt.MapFrom(src => src.Instruction));
            CreateMap<PlantInfo, GetPlantGuideStepByPlantIdResponse>()
            .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.PlantInfoName, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.PlantInfoScientificName, opt => opt.MapFrom(src => src.ScientificName))
            .ForMember(dest => dest.PlantInfoIdealSoil, opt => opt.MapFrom(src => src.IdealSoil))
            // السطر ده سحري: هيقوم بتحويل قائمة GuideSteps تلقائياً باستخدام المابينج رقم 1
            .ForMember(dest => dest.Steps, opt => opt.MapFrom(src => src.GuideSteps));

        }
    }
}