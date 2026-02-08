using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace Core.Mapping.CropMapping
{
    public partial class CropProfile :Profile
    {
        public CropProfile()
        {
            CreateCropMapping();
            UpdateCropMapping();
            GetCropMapping();
        }
    }
}