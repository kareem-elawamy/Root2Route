using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Features.PlantInfo.Commands.Models;
using Domain.Models;

namespace Core.Mapping.PlantInfoMapping
{
    public partial class PalntInfoProfile
    {
        public void EditPlantInfoMapping()
        {
            CreateMap<EditPlantInfoCommand, PlantInfo>()
             .ForMember(dest => dest.ImageUrl, opt => opt.Ignore());
        }

    }
}