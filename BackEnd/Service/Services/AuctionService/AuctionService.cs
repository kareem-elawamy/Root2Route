using Domain.Enums;
using Domain.Models;
using Infrastructure.Repositories.AuctionRepository;
using Infrastructure.Repositories.OrganizationMemberRepository;
using Infrastructure.Repositories.OrderRepository;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Service.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Services.AuctionService
{
    public class AuctionService : IAuctionService
    {
        private readonly IAuctionRepository _auctionRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrganizationMemberRepository _organizationMemberRepository;
        private readonly IHubContext<AuctionHub> _hubContext;

        public AuctionService(IAuctionRepository auctionRepository, IOrderRepository orderRepository, IOrganizationMemberRepository organizationMemberRepository, IHubContext<AuctionHub> hubContext)
        {
            _auctionRepository = auctionRepository;
            _orderRepository = orderRepository;
            _organizationMemberRepository = organizationMemberRepository;
            _hubContext = hubContext;
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

        public async Task<List<Auction>> GetActiveAuctionsAsync(AuctionFilter? filter = null, int pageNumber = 1, int pageSize = 10)
        {
            var query = _auctionRepository.GetTableNoTracking()
                .Include(a => a.product)
                .Include(a => a.HighestBidder)
                .Where(a => a.Status == AuctionStatus.Active);

            if (filter != null)
            {
                if (!string.IsNullOrEmpty(filter.SearchTerm))
                    query = query.Where(a => a.Title.Contains(filter.SearchTerm) || (a.product != null && a.product.Name.Contains(filter.SearchTerm)));
                
                if (filter.MinPrice.HasValue)
                    query = query.Where(a => a.CurrentHighestBid >= filter.MinPrice.Value || (a.CurrentHighestBid == 0 && a.StartPrice >= filter.MinPrice.Value));
                
                if (filter.MaxPrice.HasValue)
                    query = query.Where(a => a.CurrentHighestBid <= filter.MaxPrice.Value || (a.CurrentHighestBid == 0 && a.StartPrice <= filter.MaxPrice.Value));
                
                // Assuming CategoryId is on Product
                // if (filter.CategoryId.HasValue)
                //    query = query.Where(a => a.product != null && a.product.CategoryId == filter.CategoryId.Value);

                if (!string.IsNullOrEmpty(filter.SortBy))
                {
                    query = filter.SortBy.ToLower() switch
                    {
                        "price_asc" => query.OrderBy(a => a.CurrentHighestBid == 0 ? a.StartPrice : a.CurrentHighestBid),
                        "price_desc" => query.OrderByDescending(a => a.CurrentHighestBid == 0 ? a.StartPrice : a.CurrentHighestBid),
                        "endtime" => query.OrderBy(a => a.EndDate),
                        _ => query.OrderByDescending(a => a.StartDate)
                    };
                }
                else
                {
                    query = query.OrderByDescending(a => a.StartDate);
                }
            }
            else
            {
                query = query.OrderByDescending(a => a.StartDate);
            }

            return await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<List<Auction>> GetCompletedAuctionsAsync(int pageNumber = 1, int pageSize = 10)
        {
            return await _auctionRepository.GetTableNoTracking()
                .Include(a => a.product)
                .Include(a => a.HighestBidder)
                .Where(a => a.Status == AuctionStatus.Completed)
                .OrderByDescending(a => a.EndDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
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
                // Broadcast ReceiveAuctionClosed to all listeners in this auction group
                await _hubContext.Clients.Group(auction.Id.ToString())
                    .SendAsync("ReceiveAuctionClosed", auction.HighestBidderId, auction.CurrentHighestBid);
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

        public async Task<string> UpdateAuctionAsync(Guid auctionId, Auction updatedData, Guid sellerId)
        {
            var auction = await _auctionRepository.GetTableAsTracking()
                .Include(a => a.Bids)
                .Include(a => a.product)
                .FirstOrDefaultAsync(a => a.Id == auctionId);
            if (auction == null) return "Not Found";
            if (auction.product?.OrganizationId == null) return "Not Found";
            var isSeller = await _organizationMemberRepository.GetTableNoTracking()
                .AnyAsync(m => m.OrganizationId == auction.product.OrganizationId && m.UserId == sellerId && m.IsActive);
            if (!isSeller) return "Unauthorized";
            if (auction.Status != AuctionStatus.Pending) return "Failed: Can only update a Pending auction";
            if (auction.Bids.Any()) return "Failed: Cannot update an auction that already has bids";

            auction.Title = updatedData.Title;
            auction.StartDate = updatedData.StartDate;
            auction.EndDate = updatedData.EndDate;
            auction.StartPrice = updatedData.StartPrice;
            auction.MinimumBidIncrement = updatedData.MinimumBidIncrement;
            auction.ReservePrice = updatedData.ReservePrice;

            await _auctionRepository.UpdateAsync(auction);
            return "Success";
        }

        public async Task<string> CancelAuctionAsync(Guid auctionId, Guid sellerId)
        {
            var auction = await _auctionRepository.GetTableAsTracking()
                .Include(a => a.product)
                .FirstOrDefaultAsync(a => a.Id == auctionId);
            if (auction == null) return "Not Found";
            if (auction.product?.OrganizationId == null) return "Not Found";
            var isSeller = await _organizationMemberRepository.GetTableNoTracking()
                .AnyAsync(m => m.OrganizationId == auction.product.OrganizationId && m.UserId == sellerId && m.IsActive);
            if (!isSeller) return "Unauthorized";
            if (auction.Status == AuctionStatus.Completed) return "Failed: Cannot cancel a completed auction";

            auction.Status = AuctionStatus.Cancelled;
            await _auctionRepository.UpdateAsync(auction);
            return "Success";
        }

        public async Task<List<Auction>> GetMyOrganizationAuctionsAsync(Guid organizationId)
        {
            return await _auctionRepository.GetTableNoTracking()
                .Include(a => a.product)
                .Include(a => a.HighestBidder)
                .Where(a => a.product != null && a.product.OrganizationId == organizationId)
                .OrderByDescending(a => a.StartDate)
                .ToListAsync();
        }

        public async Task<List<Auction>> GetMyWonAuctionsAsync(Guid userId)
        {
            return await _auctionRepository.GetTableNoTracking()
                .Include(a => a.product)
                .Include(a => a.HighestBidder)
                .Where(a => a.Status == AuctionStatus.Completed && a.HighestBidderId == userId)
                .OrderByDescending(a => a.EndDate)
                .ToListAsync();
        }

        public async Task<List<Auction>> GetMyParticipatedAuctionsAsync(Guid userId)
        {
            return await _auctionRepository.GetTableNoTracking()
                .Include(a => a.product)
                .Include(a => a.HighestBidder)
                .Include(a => a.Bids)
                .Where(a => a.Status == AuctionStatus.Active && a.Bids.Any(b => b.BidderId == userId))
                .OrderBy(a => a.EndDate)
                .ToListAsync();
        }

        public async Task<string> CheckoutWonAuctionAsync(Guid auctionId, Guid userId, string shippingAddress, string paymentMethod)
        {
            var auction = await _auctionRepository.GetTableAsTracking()
                .Include(a => a.product)
                .FirstOrDefaultAsync(a => a.Id == auctionId);

            if (auction == null) return "Not Found";
            if (auction.Status != AuctionStatus.Completed) return "Failed: Auction is not completed";
            if (auction.HighestBidderId != userId) return "Unauthorized: You are not the winner";

            // Check if already checked out? 
            // In a real system, we might have an 'IsOrdered' flag on Auction or check existing orders.

            using var transaction = await _auctionRepository.BeginTransactionAsync();
            try
            {
                var order = new Order
                {
                    BuyerId = userId,
                    OrderDate = DateTime.UtcNow,
                    TotalAmount = auction.CurrentHighestBid,
                    Status = OrderStatus.Processing, // Default status for a new order
                    ReceiverName = "Auction Winner", // Placeholder, usually from user profile or command
                    ReceiverPhone = "0000000000",   // Placeholder
                    ShippingCity = "City",         // Placeholder or parsed from shippingAddress
                    ShippingStreet = shippingAddress,
                    PaymentMethod = Enum.Parse<PaymentMethod>(paymentMethod, true),
                    PaymentStatus = PaymentStatus.Pending,
                    OrderItems = new List<OrderItem>
                    {
                        new OrderItem
                        {
                            productid = auction.productid,
                            Quantity = 1,
                            UnitPrice = auction.CurrentHighestBid
                        }
                    }
                };

                await _orderRepository.AddAsync(order);
                
                // Mark auction as processed?
                // auction.Status = AuctionStatus.Ordered; // If such status exists

                await transaction.CommitAsync();
                return order.Id.ToString();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return $"Error: {ex.Message}";
            }
        }
    }
}