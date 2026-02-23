using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enums;
using Infrastructure.Repositories.AuctionRepository;
using Infrastructure.Repositories.BidRepository;
using Infrastructure.Repositories.ProductRepository;

namespace Service.Services.AuctionService
{
    public class AuctionService : IAuctionService
    {
        private readonly IAuctionRepository _auctionRepository;
        private readonly IProductRepository _productRepository;
        private readonly IBidRepository _bidRepository;
        public AuctionService(IAuctionRepository auctionRepository, IProductRepository productRepository, IBidRepository bidRepository)
        {
            _bidRepository = bidRepository;
            _productRepository = productRepository;
            _auctionRepository = auctionRepository;
        }
        public async Task<string> CreateAuctionAsync(Guid marketItemId, int durationInHours)
        {
            var item = await _productRepository.GetByIdAsync(marketItemId);
            if (item == null || !item.IsAvailableForAuction) return "Item not found or not for auction";
            var auction = new Auction
            {
                Id = Guid.NewGuid(),
                MarketItemId = marketItemId,
                Title = $"Auction for {item.Name}",
                StartPrice = item.StartBiddingPrice,
                CurrentHighestBid = item.StartBiddingPrice,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddHours(durationInHours),
                Status = AuctionStatus.Ongoing
            };
            await _auctionRepository.AddAsync(auction);
            return "Auction Created Successfully";
        }

        public async Task<string> PlaceBidAsync(Guid auctionId, Guid bidderId, decimal amount)
        {
            var auction = await _auctionRepository.GetByIdAsync(auctionId);
            if (auction == null) return "Auction not found";
            if (auction.EndDate < DateTime.UtcNow) return "Auction has expired";
            if (amount <= auction.CurrentHighestBid)
                return "Bid must be higher than the current highest bid";
            var newBid = new Bid
            {
                Id = Guid.NewGuid(),
                AuctionId = auctionId,
                BidderId = bidderId,
                Amount = amount,
                BidTime = DateTime.UtcNow
            };

            await _bidRepository.AddAsync(newBid);
            auction.CurrentHighestBid = amount;
            await _auctionRepository.UpdateAsync(auction);

            return "Bid Placed Successfully";
        }
    }
}