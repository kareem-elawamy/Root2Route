using Domain.Enums;
using Domain.Models;
using Infrastructure.Repositories.AuctionRepository;
using Infrastructure.Repositories.OrganizationMemberRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Services.AuctionService
{
    public class AuctionService : IAuctionService
    {
        private readonly IAuctionRepository _auctionRepository;
        private readonly IOrganizationMemberRepository _organizationMemberRepository;

        public AuctionService(IAuctionRepository auctionRepository, IOrganizationMemberRepository organizationMemberRepository)
        {
            _auctionRepository = auctionRepository;
            _organizationMemberRepository = organizationMemberRepository;
        }

        public async Task<string> CreateAuctionAsync(Auction auction)
        {
            auction.Status = AuctionStatus.Pending; 
            if (auction.StartDate <= DateTime.UtcNow)
            {
                auction.Status = AuctionStatus.Active;
            }
            
            var result = await _auctionRepository.AddAsync(auction);
            return result != null ? "Success" : "Failed";
        }

        public async Task<string> PlaceBidAsync(Guid auctionId, Guid bidderId, decimal amount)
        {
            var auction = await _auctionRepository.GetTableAsTracking()
                .Include(a => a.product)
                .Include(a => a.Bids)
                .FirstOrDefaultAsync(a => a.Id == auctionId);
            
            if (auction == null) return "Failed: Auction not found";
            if (auction.Status != AuctionStatus.Active) return "Failed: Auction is not active";
            if (auction.EndDate <= DateTime.UtcNow) return "Failed: Auction has ended";

            // Shill Bidding Check
            if (auction.product != null)
            {
                var isSeller = await _organizationMemberRepository.GetTableNoTracking()
                    .AnyAsync(m => m.OrganizationId == auction.product.OrganizationId && m.UserId == bidderId && m.IsActive);
                if (isSeller) return "Failed: You cannot bid on your own organization's auction.";
            }

            var minRequiredBid = auction.CurrentHighestBid == 0 
                ? auction.StartPrice 
                : auction.CurrentHighestBid + auction.MinimumBidIncrement;
                
            if (amount < minRequiredBid)
                return $"Failed: Minimum bid is {minRequiredBid}";

            using var transaction = await _auctionRepository.BeginTransactionAsync();
            try
            {
                var bid = new Bid
                {
                    AuctionId = auctionId,
                    BidderId = bidderId,
                    Amount = amount,
                    BidTime = DateTime.UtcNow
                };

                auction.CurrentHighestBid = amount;
                auction.HighestBidderId = bidderId;
                auction.Bids.Add(bid);

                await _auctionRepository.UpdateAsync(auction);
                await transaction.CommitAsync();
                return "Success";
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return "Failed: An error occurred while placing the bid";
            }
        }

        public async Task<Auction?> GetAuctionByIdAsync(Guid id)
        {
            return await _auctionRepository.GetTableNoTracking()
                .Include(a => a.product)
                .Include(a => a.HighestBidder)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<Auction>> GetActiveAuctionsAsync()
        {
            return await _auctionRepository.GetTableNoTracking()
                .Where(a => a.Status == AuctionStatus.Active)
                .ToListAsync();
        }

        public async Task FinalizeExpiredAuctionsAsync()
        {
            var expiredAuctions = await _auctionRepository.GetTableAsTracking()
                .Where(a => a.Status == AuctionStatus.Active && a.EndDate <= DateTime.UtcNow)
                .ToListAsync();

            foreach (var auction in expiredAuctions)
            {
                auction.Status = AuctionStatus.Completed;
                await _auctionRepository.UpdateAsync(auction);
            }
        }

        public async Task<List<Bid>> GetBidsForAuctionAsync(Guid auctionId)
        {
            var auction = await _auctionRepository.GetTableNoTracking()
                .Include(a => a.Bids)
                    .ThenInclude(b => b.Bidder)
                .FirstOrDefaultAsync(a => a.Id == auctionId);
                
            if (auction == null) return new List<Bid>();
            return auction.Bids.OrderByDescending(b => b.BidTime).ToList();
        }
    }
}