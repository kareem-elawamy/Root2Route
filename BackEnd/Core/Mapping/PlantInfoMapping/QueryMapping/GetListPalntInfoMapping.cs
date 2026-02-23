using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Core.Features.PlantInfo.Queries.Result;

namespace Core.Mapping.PlantInfoMapping
{
    public partial class PalntInfoProfile
    {
        public void ConfigureGetListPalntInfoMapping()
        {
            CreateMap<PlantInfo, PlantInfoListResponse>();
        }

    }
}