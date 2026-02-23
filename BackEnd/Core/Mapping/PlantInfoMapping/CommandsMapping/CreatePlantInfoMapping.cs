using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Features.PlantInfo.Commands.Models;
using Domain.Models;
using static Domain.Models.PlantInfo;

namespace Core.Mapping.PlantInfoMapping
{
    public partial class PalntInfoProfile
    {

        public void CreatePlantInfoMapping()
        {
            CreateMap<CreatePlantInfoCommand, PlantInfo>()
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())  ;
        }

    }
}