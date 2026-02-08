using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Services.ProductService
{
    public interface IProductService
    {
        Task<string> CreateProductFromCropAsync(Product product);


    }

}