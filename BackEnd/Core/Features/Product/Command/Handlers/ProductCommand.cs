using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Features.Product.Command.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Service.Services.CropService;
using Service.Services.ProductService;

namespace Core.Features.Product.Command.Handlers
{
    public class ProductCommand : ResponseHandler, IRequestHandler<CreateProductFromCropCommand, Response<string>>
    {
        private readonly ICropService _cropService;
        private readonly IProductService _productService;
        private readonly IMapper _mapper;
        public ProductCommand(ICropService cropService,
                                        IProductService productService,
                                        IMapper mapper)
        {
            _cropService = cropService;
            _productService = productService;
            _mapper = mapper;
        }
        public async Task<Response<string>> Handle(CreateProductFromCropCommand request, CancellationToken cancellationToken)
        {
            var crop = await _cropService.GetCropWithDetailsAsync(request.CropId);
            if (crop == null)
                return NotFound<string>("المحصول غير موجود أو تم حذفه.");

            var productEntity = _mapper.Map<Domain.Models.Product>(crop);

            // 3. تطعيم الـ Entity ببيانات الـ Command (السعر والكمية)
            productEntity.DirectSalePrice = request.Price;
            productEntity.StockQuantity = request.Quantity;
            productEntity.IsAvailableForAuction = request.IsAuction;
            productEntity.StartBiddingPrice = request.StartBiddingPrice ?? 0;
            productEntity.IsAvailableForDirectSale = !request.IsAuction;
            if (!string.IsNullOrEmpty(request.Description))
                productEntity.Description = request.Description;
            var result = await _productService.CreateProductFromCropAsync(productEntity);

            if (result == "Success")
                return Success("تم إدراج المحصول في السوق بنجاح");

            return BadRequest<string>("فشل في إدراج المحصول، حاول مرة أخرى.");

        }
    }
}