using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Repositories.CropRepository;
using Infrastructure.Repositories.ProductRepository;

namespace Service.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICropRepository _cropRepository;
        public ProductService(IProductRepository productRepository, ICropRepository cropRepository)
        {
            _cropRepository = cropRepository;
            _productRepository = productRepository;
        }

        public async Task<string> CreateProductFromCropAsync(Product product)
        {
            try
            {
                // إضافة المنتج الجديد لقاعدة البيانات
                await _productRepository.AddAsync(product);

                // (اختياري) ممكن هنا تحدث حالة الـ Crop الأصلي إنه خلاص بقى منتج
                if (product.SourceCropId.HasValue)
                {
                    var crop = await _cropRepository.GetByIdAsync(product.SourceCropId.Value);
                    if (crop != null) { crop.IsConvertedToProduct = true; await _cropRepository.UpdateAsync(crop); }
                }

                return "Success";
            }
            catch (Exception ex)
            {
                // يفضل تسجيل الخطأ (Logging) هنا
                return "Failed";
            }
        }
    }
}