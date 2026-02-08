using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace Core.Mapping.ProductMapping
{
    public partial class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CropToProduct();
        }
    }
}