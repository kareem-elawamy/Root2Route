using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Enums;
using Domain.Models;

namespace Service.Services.ProductService
{
    public interface IProductService
    {
        Task<Product> AddProductAsync(Product product);

        Task<Product?> GetProductByIdAsync(Guid productId);

        Task<List<Product>> GetAllProductsAsync();
        Task<(List<Product> Products, int TotalCount)> GetPaginatedProductsAsync(int pageNumber, int pageSize, string? search = null, ProductType? type = null, ProductStatus? status = ProductStatus.Approved);
        Task<(List<Product> Products, int TotalCount)> GetPaginatedProductsByOrgIdAsync(Guid organizationId, int pageNumber, int pageSize);

        Task UpdateProductAsync(Product product);

        Task DeleteProductAsync(Product product);

        Task<bool> IsBarcodeExistAsync(string barcode);
        Task<bool> ChangeProductStatusAsync(Guid productId, ProductStatus newStatus, string? rejectionReason = null);
    }
}