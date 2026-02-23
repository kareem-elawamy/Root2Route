using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Features.PlantInfo.Queries.Models;
using Core.Features.PlantInfo.Queries.Result;
using Domain.Models;

namespace Core.Mapping.PlantInfoMapping
{
    public partial class PalntInfoProfile
    {
        public void GetPantInfoPaginationMapping()
        {
            CreateMap<PlantInfo, GetPlantInfoPaginatedListResponse>();
        }

    }
}