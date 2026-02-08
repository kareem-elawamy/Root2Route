using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Features.Crop.Query.Result;
using Domain.Models;

namespace Core.Mapping.CropMapping
{
    public partial class CropProfile
    {
        public void GetCropMapping()
        {
            CreateMap<Crop, CropResponse>()
            .ForMember(dest => dest.FarmName, opt => opt.MapFrom(src => src.Farm.Name)) // مثال لجلب اسم المزرعة
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString())) // تحويل الـ Enum لنص

            .ForMember(dest => dest.PlantName, opt => opt.MapFrom(src => src.PlantInfo.Name))
                .ForMember(dest => dest.ScientificName, opt => opt.MapFrom(src => src.PlantInfo.ScientificName))
                .ForMember(dest => dest.PlantImageUrl, opt => opt.MapFrom(src => src.PlantInfo.ImageUrl));
        }
    }
}