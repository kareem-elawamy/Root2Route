using System;
using System.Collections.Generic; // عشان List
using Core.Base;
using MediatR;
using Domain.Enums;
using Microsoft.AspNetCore.Http; // عشان IFormFile

namespace Core.Features.Product.Command.Models
{
    public class AddProductCommand : IRequest<Response<string>>
    {
        public Guid OrganizationId { get; set; }

        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int StockQuantity { get; set; }
        public List<IFormFile>? Images { get; set; }
        public bool IsAvailableForDirectSale { get; set; }
        public decimal DirectSalePrice { get; set; }

        public bool IsAvailableForAuction { get; set; }
        public decimal StartBiddingPrice { get; set; }

        public string? Barcode { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public WeightUnit? WeightUnit { get; set; }
        public ProductType ProductType { get; set; }
    }
}