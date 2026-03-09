using System;
using Core.Base;
using Domain.Enums;
using MediatR;

namespace Core.Features.Product.Command.Models
{
    public class UpdateProductCommand : IRequest<Response<string>>
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int StockQuantity { get; set; }
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