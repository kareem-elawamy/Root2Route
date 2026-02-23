using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace Core.Mapping.PlantInfoMapping
{
    public partial class PalntInfoProfile : Profile
    {
        public PalntInfoProfile()
        {
            ConfigureGetListPalntInfoMapping();
            CreatePlantInfoMapping();
            EditPlantInfoMapping();
            GetPantInfoPaginationMapping();
        }
    }
}