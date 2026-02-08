using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace Core.Mapping.FarmMapping
{
    public partial class FarmProfile : Profile
    {
        public FarmProfile()
        {
            CreateFarm();
        }
    }
}