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
        Task<List<Auction>> GetActiveAuctionsAsync(AuctionFilter? filter = null, int pageNumber = 1, int pageSize = 10);
        Task<List<Auction>> GetCompletedAuctionsAsync(int pageNumber = 1, int pageSize = 10);
        Task FinalizeExpiredAuctionsAsync();
        Task<List<Bid>> GetBidsForAuctionAsync(Guid auctionId);
        Task<string> UpdateAuctionAsync(Guid auctionId, Auction updatedData, Guid sellerId);
        Task<string> CancelAuctionAsync(Guid auctionId, Guid sellerId);
        Task<List<Auction>> GetMyOrganizationAuctionsAsync(Guid organizationId);
        Task<List<Auction>> GetMyWonAuctionsAsync(Guid userId);
        Task<List<Auction>> GetMyParticipatedAuctionsAsync(Guid userId);
        Task<string> CheckoutWonAuctionAsync(Guid auctionId, Guid userId, string shippingAddress, string paymentMethod);
    }

    public class AuctionFilter
    {
        public string? SearchTerm { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public Guid? CategoryId { get; set; }
        public string? SortBy { get; set; }
    }
}