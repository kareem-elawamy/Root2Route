using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Features.Product.Command.Models;
using Domain.Models;

namespace Core.Mapping.ProductMapping
{
    public partial class ProductProfile
    {
        public void CropToProduct()
        {
            CreateMap<Crop, Product>()
     .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
     .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.PlantInfo.Name))
     .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.PlantInfo.ImageUrl))
     .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.PlantInfo.Description))
     .ForMember(dest => dest.OrganizationId, opt => opt.MapFrom(src => src.Farm.OrganizationId))
     .ForMember(dest => dest.SourceCropId, opt => opt.MapFrom(src => src.Id))
     .ForMember(dest => dest.WeightUnit, opt => opt.MapFrom(src => src.YieldUnit));
        }
    }
}