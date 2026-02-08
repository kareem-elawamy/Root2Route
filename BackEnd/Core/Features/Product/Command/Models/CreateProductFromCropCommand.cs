using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Base;
using MediatR;

namespace Core.Features.Product.Command.Models
{
    public class CreateProductFromCropCommand : IRequest<Response<string>>
    {
        public Guid CropId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public bool IsAuction { get; set; }
        public decimal? StartBiddingPrice { get; set; }
        public string? Description { get; set; } // وصف إضافي للماركت غير وصف النبات
    }
}