using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Base;
using Core.Features.Product.Command.Models;
using MediatR;
using Service.Services.ProductService;
using Service.Services.FileService; // ضفنا الـ FileService
using Domain.Models; // عشان Product و ProductImage

namespace Core.Features.Product.Command.Handlers
{
    public class AddProductCommandHandler : IRequestHandler<AddProductCommand, Response<string>>
    {
        private readonly IProductService _productService;
        private readonly IFileService _fileService; // 👈 إضافة الـ File Service

        // 👇 التعديل في الكونستركتور
        public AddProductCommandHandler(IProductService productService, IFileService fileService)
        {
            _productService = productService;
            _fileService = fileService;
        }

        public async Task<Response<string>> Handle(AddProductCommand request, CancellationToken cancellationToken)
        {
            // 1. فحص الباركود
            if (!string.IsNullOrWhiteSpace(request.Barcode))
            {
                var isExist = await _productService.IsBarcodeExistAsync(request.Barcode);
                if (isExist) return new Response<string>("هذا الباركود مسجل لمنتج آخر مسبقاً");
            }

            // 2. 👈 رفع الصور باستخدام الـ FileService
            var productImages = new List<ProductImage>();

            if (request.Images != null && request.Images.Any())
            {
                // بنبعت اسم الفولدر اللي عايزين نحفظ فيه الصور، مثلاً "Products"
                var uploadedUrls = await _fileService.UploadImagesAsync("Products", request.Images);

                // تحويل اللينكات لـ Entities
                for (int i = 0; i < uploadedUrls.Count; i++)
                {
                    productImages.Add(new ProductImage
                    {
                        ImageUrl = uploadedUrls[i],
                        IsMain = (i == 0) // بنخلي أول صورة مرفوعة هي الصورة الرئيسية للمنتج
                    });
                }
            }

            // 3. Mapping Product
            var newProduct = new Domain.Models.Product
            {
                OrganizationId = request.OrganizationId,
                Name = request.Name,
                Description = request.Description,
                StockQuantity = request.StockQuantity,

                IsAvailableForDirectSale = request.IsAvailableForDirectSale,
                DirectSalePrice = request.IsAvailableForDirectSale ? request.DirectSalePrice : 0,

                IsAvailableForAuction = request.IsAvailableForAuction,
                StartBiddingPrice = request.IsAvailableForAuction ? request.StartBiddingPrice : 0,

                Barcode = request.Barcode,
                ExpiryDate = request.ExpiryDate,
                WeightUnit = request.WeightUnit,
                ProductType = request.ProductType,

                // 👈 ربط الصور بالمنتج
                Images = productImages
            };

            // 4. الحفظ في الداتابيز
            await _productService.AddProductAsync(newProduct);

            return new Response<string>("تمت إضافة المنتج بنجاح") { Succeeded = true };
        }
    }
}