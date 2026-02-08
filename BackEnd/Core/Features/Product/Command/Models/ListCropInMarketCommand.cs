using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Base;
using MediatR;

namespace Core.Features.Product.Command.Models
{
    public class ListCropInMarketCommand : IRequest<Response<Guid>>
    {
        public Guid CropId { get; set; }
        public decimal SalePrice { get; set; }
        public bool IsAuction { get; set; }
        public decimal? StartBiddingPrice { get; set; }
        public int QuantityToList { get; set; }
        // ممكن المزارع يغير الوصف أو يضيف صور غير اللي في الـ Crop
        public string? MarketDescription { get; set; }
    }
}