using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Services.AuctionService
{
    public interface IAuctionService
    {
        Task<string> CreateAuctionAsync(Guid marketItemId, int durationInHours);
        Task<string> PlaceBidAsync(Guid auctionId, Guid bidderId, decimal amount);
    }
}