using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Features.Product.Command.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Service.Services.ProductService;

namespace Core.Features.Product.Command.Handlers
{
    public class ProductCommand : ResponseHandler
    {

        private readonly IProductService _productService;
        private readonly IMapper _mapper;
        public ProductCommand(
                                        IProductService productService,
                                        IMapper mapper)
        {
            _productService = productService;
            _mapper = mapper;
        }

    }
}