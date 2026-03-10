using Domain.Enums;
using Domain.Models;
using Infrastructure.Repositories.ProductRepository;
using Microsoft.EntityFrameworkCore;
using Mscc.GenerativeAI.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Product> AddProductAsync(Product product)
        {
            return await _productRepository.AddAsync(product);
        }

        public async Task<Product?> GetProductByIdAsync(Guid productId)
        {
            return await _productRepository.GetTableNoTracking()
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == productId);
        }
        public async Task<bool> ChangeProductStatusAsync(Guid productId, ProductStatus newStatus, string? rejectionReason = null)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null) return false;

            product.Status = newStatus;
            if (newStatus == ProductStatus.Rejected)
            {
                product.RejectionReason = rejectionReason;
            }
            else
            {
                product.RejectionReason = null; // لو وافقنا عليه نشيل سبب الرفض لو كان موجود
            }

            await _productRepository.UpdateAsync(product);
            return true;
        }

        public async Task<(List<Product> Products, int TotalCount)> GetPaginatedProductsAsync(int pageNumber, int pageSize, string? search = null, ProductType? type = null, ProductStatus? status = ProductStatus.Approved)
        {
            var query = _productRepository.GetTableNoTracking()
                .Include(p => p.Images)
                .AsQueryable();
            if (status.HasValue)
            {
                query = query.Where(p => p.Status == status.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p => p.Name.Contains(search) ||
                                        (p.Description != null && p.Description.Contains(search)));
            }

            if (type.HasValue)
            {
                query = query.Where(p => p.ProductType == type.Value);
            }

            var totalCount = await query.CountAsync();

            var products = await query
                .OrderByDescending(p => p.Id) 
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (products, totalCount);
        }

        public async Task<(List<Product> Products, int TotalCount)> GetPaginatedProductsByOrgIdAsync(Guid organizationId, int pageNumber, int pageSize)
        {
            var query = _productRepository.GetTableNoTracking()
                .Include(p => p.Images)
                .Where(p => p.OrganizationId == organizationId)
                .AsQueryable();

            var totalCount = await query.CountAsync();

            var products = await query
                .OrderByDescending(p => p.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (products, totalCount);
        }


        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _productRepository.GetTableNoTracking()
                .Include(p => p.Images)
                .ToListAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            await _productRepository.UpdateAsync(product);
        }

        public async Task DeleteProductAsync(Product product)
        {
            await _productRepository.DeleteAsync(product);
        }

        public async Task<bool> IsBarcodeExistAsync(string barcode)
        {
            if (string.IsNullOrWhiteSpace(barcode)) return false;

            return await _productRepository.GetTableNoTracking()
                .AnyAsync(p => p.Barcode == barcode);
        }
    }
}