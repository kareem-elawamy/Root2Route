using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Features.Farm.Commands.Models;
using Domain.Models;

namespace Core.Mapping.FarmMapping
{
    public partial class FarmProfile
    {
        public void CreateFarm()
        {
            CreateMap<CreateFarmCommand, Farm>();
        }
    }
}