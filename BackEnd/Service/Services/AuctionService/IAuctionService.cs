using Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Services.AuctionService
{
    public interface IAuctionService
    {
        Task<string> CreateAuctionAsync(Auction auction);
        Task<string> PlaceBidAsync(Guid auctionId, Guid bidderId, decimal amount);
        Task<Auction?> GetAuctionByIdAsync(Guid id);
        Task<List<Auction>> GetActiveAuctionsAsync();
        Task FinalizeExpiredAuctionsAsync();
        Task<List<Bid>> GetBidsForAuctionAsync(Guid auctionId);
    }
}