using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Features.Crop.Command.Models;
using Domain.Models;

namespace Core.Mapping.CropMapping
{
    public partial class CropProfile
    {
        public void UpdateCropMapping()
        {
            CreateMap<UpdateCropCommand, Crop>();
        }

    }
}