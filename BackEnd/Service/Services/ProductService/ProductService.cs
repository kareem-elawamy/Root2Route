using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Repositories.ProductRepository;

namespace Service.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<string> CreateProductFromCropAsync(Product product)
        {
            try
            {
                // إضافة المنتج الجديد لقاعدة البيانات
                await _productRepository.AddAsync(product);

                // (اختياري) ممكن هنا تحدث حالة الـ Crop الأصلي إنه خلاص بقى منتج
                

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