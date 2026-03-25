using Domain.Enums;
using Domain.Models;
using Infrastructure.Repositories.AuctionRepository;
using Infrastructure.Repositories.OrganizationMemberRepository;
using Infrastructure.Repositories.OrderRepository;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Service.Hubs;
using Service.Services.NotificationService;
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
        private readonly INotificationService _notificationService;

        public AuctionService(IAuctionRepository auctionRepository, IOrderRepository orderRepository, IOrganizationMemberRepository organizationMemberRepository, IHubContext<AuctionHub> hubContext, INotificationService notificationService)
        {
            _auctionRepository = auctionRepository;
            _orderRepository = orderRepository;
            _organizationMemberRepository = organizationMemberRepository;
            _hubContext = hubContext;
            _notificationService = notificationService;
        }

        public async Task<Guid> CreateAuctionAsync(Auction auction)
        {
            auction.Status = AuctionStatus.Upcoming; 
            if (auction.StartDate <= DateTime.UtcNow)
            {
                auction.Status = AuctionStatus.Ongoing;
            }
            
            await _auctionRepository.AddAsync(auction);
            return auction.Id;
        }

        public async Task PlaceBidAsync(Guid auctionId, Guid bidderId, decimal amount)
        {
            var auction = await _auctionRepository.GetTableAsTracking()
                .Include(a => a.Product)
                .Include(a => a.Bids)
                .FirstOrDefaultAsync(a => a.Id == auctionId);
            
            if (auction == null) throw new KeyNotFoundException("Auction not found");
            if (auction.Status != AuctionStatus.Ongoing) throw new InvalidOperationException("Auction is not active");
            if (auction.EndDate <= DateTime.UtcNow) throw new InvalidOperationException("Auction has ended");

            // Shill Bidding Check
            if (auction.Product != null)
            {
                var isSeller = await _organizationMemberRepository.GetTableNoTracking()
                    .AnyAsync(m => m.OrganizationId == auction.Product.OrganizationId && m.UserId == bidderId && m.IsActive);
                if (isSeller) throw new UnauthorizedAccessException("You cannot bid on your own organization's auction.");
            }

            var minRequiredBid = auction.CurrentHighestBid == 0 
                ? auction.StartPrice 
                : auction.CurrentHighestBid + auction.MinimumBidIncrement;
                
            if (amount < minRequiredBid)
                throw new InvalidOperationException($"Minimum bid is {minRequiredBid}");

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

                // Notify the previously outbid bidder (if any)
                var previousHighBidder = auction.Bids
                    .Where(b => b.BidderId != bidderId)
                    .OrderByDescending(b => b.BidTime)
                    .Select(b => b.BidderId)
                    .FirstOrDefault();

                if (previousHighBidder != Guid.Empty && previousHighBidder != bidderId)
                {
                    try
                    {
                        await _notificationService.SendPushNotificationAsync(
                            previousHighBidder,
                            "You've been outbid! 🔔",
                            $"Someone placed a higher bid on '{auction.Product?.Name ?? "this auction"}'. Place a new bid to stay in the lead!",
                            auction.Id.ToString());
                    }
                    catch { }
                }
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Auction?> GetAuctionByIdAsync(Guid id)
        {
            return await _auctionRepository.GetTableNoTracking()
                .Include(a => a.Product)
                .Include(a => a.HighestBidder)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<Auction>> GetActiveAuctionsAsync(AuctionFilter? filter = null, int pageNumber = 1, int pageSize = 10)
        {
            var query = _auctionRepository.GetTableNoTracking()
                .Include(a => a.Product)
                .Include(a => a.HighestBidder)
                .Where(a => a.Status == AuctionStatus.Ongoing);

            if (filter != null)
            {
                if (!string.IsNullOrEmpty(filter.SearchTerm))
                    query = query.Where(a => a.Title.Contains(filter.SearchTerm) || (a.Product != null && a.Product.Name.Contains(filter.SearchTerm)));
                
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
                .Include(a => a.Product)
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
                .Include(a => a.Product)
                .Where(a => a.Status == AuctionStatus.Ongoing && a.EndDate <= DateTime.UtcNow)
                .ToListAsync();

            foreach (var auction in expiredAuctions)
            {
                auction.Status = AuctionStatus.Completed;
                await _auctionRepository.UpdateAsync(auction);
                await _hubContext.Clients.Group(auction.Id.ToString())
                    .SendAsync("ReceiveAuctionClosed", auction.HighestBidderId, auction.CurrentHighestBid);

                // Notify winner
                if (auction.HighestBidderId.HasValue && auction.HighestBidderId != Guid.Empty)
                {
                    try
                    {
                        await _notificationService.SendPushNotificationAsync(
                            auction.HighestBidderId.Value,
                            "You Won the Auction! 🏆",
                            $"Congratulations! You won the auction for '{auction.Product?.Name ?? auction.Title}'. Please proceed to checkout.",
                            auction.Id.ToString());
                    }
                    catch { }

                    // Notify seller org members
                    if (auction.Product?.OrganizationId != null)
                    {
                        try
                        {
                            var sellerMembers = await _organizationMemberRepository.GetTableNoTracking()
                                .Where(m => m.OrganizationId == auction.Product.OrganizationId && m.IsActive)
                                .ToListAsync();

                            foreach (var member in sellerMembers)
                            {
                                await _notificationService.SendPushNotificationAsync(
                                    member.UserId,
                                    "Auction Completed 🎉",
                                    $"Your auction for '{auction.Product.Name}' was won. Await the buyer's checkout.",
                                    auction.Id.ToString());
                            }
                        }
                        catch { }
                    }
                }
                else
                {
                    // No bids — notify seller org
                    if (auction.Product?.OrganizationId != null)
                    {
                        try
                        {
                            var sellerMembers = await _organizationMemberRepository.GetTableNoTracking()
                                .Where(m => m.OrganizationId == auction.Product.OrganizationId && m.IsActive)
                                .ToListAsync();

                            foreach (var member in sellerMembers)
                            {
                                await _notificationService.SendPushNotificationAsync(
                                    member.UserId,
                                    "Auction Ended with No Bids",
                                    $"Your auction for '{auction.Product.Name}' ended with no bids.",
                                    auction.Id.ToString());
                            }
                        }
                        catch { }
                    }
                }
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

        public async Task UpdateAuctionAsync(Guid auctionId, Auction updatedData, Guid sellerId)
        {
            var auction = await _auctionRepository.GetTableAsTracking()
                .Include(a => a.Bids)
                .Include(a => a.Product)
                .FirstOrDefaultAsync(a => a.Id == auctionId);
            if (auction == null) throw new KeyNotFoundException("Auction not found");
            if (auction.Product?.OrganizationId == null) throw new KeyNotFoundException("Product Organization not found");
            var isSeller = await _organizationMemberRepository.GetTableNoTracking()
                .AnyAsync(m => m.OrganizationId == auction.Product.OrganizationId && m.UserId == sellerId && m.IsActive);
            if (!isSeller) throw new UnauthorizedAccessException("Only the auction owner can update it");
            if (auction.Status != AuctionStatus.Upcoming) throw new InvalidOperationException("Can only update an Upcoming auction");
            if (auction.Bids.Any()) throw new InvalidOperationException("Cannot update an auction that already has bids");

            auction.Title = updatedData.Title;
            auction.StartDate = updatedData.StartDate;
            auction.EndDate = updatedData.EndDate;
            auction.StartPrice = updatedData.StartPrice;
            auction.MinimumBidIncrement = updatedData.MinimumBidIncrement;
            auction.ReservePrice = updatedData.ReservePrice;

            await _auctionRepository.UpdateAsync(auction);
        }

        public async Task CancelAuctionAsync(Guid auctionId, Guid sellerId)
        {
            var auction = await _auctionRepository.GetTableAsTracking()
                .Include(a => a.Product)
                .FirstOrDefaultAsync(a => a.Id == auctionId);
            if (auction == null) throw new KeyNotFoundException("Auction not found");
            if (auction.Product?.OrganizationId == null) throw new KeyNotFoundException("Product Organization not found");
            var isSeller = await _organizationMemberRepository.GetTableNoTracking()
                .AnyAsync(m => m.OrganizationId == auction.Product.OrganizationId && m.UserId == sellerId && m.IsActive);
            if (!isSeller) throw new UnauthorizedAccessException("Only the auction owner can cancel it");
            if (auction.Status == AuctionStatus.Completed) throw new InvalidOperationException("Cannot cancel a completed auction");

            auction.Status = AuctionStatus.Cancelled;
            await _auctionRepository.UpdateAsync(auction);
        }

        public async Task<List<Auction>> GetMyOrganizationAuctionsAsync(Guid organizationId)
        {
            return await _auctionRepository.GetTableNoTracking()
                .Include(a => a.Product)
                .Include(a => a.HighestBidder)
                .Where(a => a.Product != null && a.Product.OrganizationId == organizationId)
                .OrderByDescending(a => a.StartDate)
                .ToListAsync();
        }

        public async Task<List<Auction>> GetMyWonAuctionsAsync(Guid userId)
        {
            return await _auctionRepository.GetTableNoTracking()
                .Include(a => a.Product)
                .Include(a => a.HighestBidder)
                .Where(a => a.Status == AuctionStatus.Completed && a.HighestBidderId == userId)
                .OrderByDescending(a => a.EndDate)
                .ToListAsync();
        }

        public async Task<List<Auction>> GetMyParticipatedAuctionsAsync(Guid userId)
        {
            return await _auctionRepository.GetTableNoTracking()
                .Include(a => a.Product)
                .Include(a => a.HighestBidder)
                .Include(a => a.Bids)
                .Where(a => a.Status == AuctionStatus.Ongoing && a.Bids.Any(b => b.BidderId == userId))
                .OrderBy(a => a.EndDate)
                .ToListAsync();
        }

        public async Task<Guid> CheckoutWonAuctionAsync(Guid auctionId, Guid userId, string receiverName, string receiverPhone, string shippingCity, string shippingAddress, string paymentMethod)
        {
            var auction = await _auctionRepository.GetTableAsTracking()
                .Include(a => a.Product)
                .FirstOrDefaultAsync(a => a.Id == auctionId);

            if (auction == null) throw new KeyNotFoundException("Auction not found");
            if (auction.Status != AuctionStatus.Completed) throw new InvalidOperationException("Auction is not completed");
            if (auction.HighestBidderId != userId) throw new UnauthorizedAccessException("You are not the winner");
            if (auction.OrderId != null) throw new InvalidOperationException("This auction has already been checked out.");

            using var transaction = await _auctionRepository.BeginTransactionAsync();
            try
            {
                var orderId = Guid.NewGuid();
                var order = new Order
                {
                    Id = orderId,
                    BuyerId = userId,
                    OrderDate = DateTime.UtcNow,
                    TotalAmount = auction.CurrentHighestBid,
                    Status = OrderStatus.Confirmed,
                    OrganizationId = auction.Product!.OrganizationId,
                    ReceiverName = receiverName,
                    ReceiverPhone = receiverPhone,
                    ShippingCity = shippingCity,
                    ShippingStreet = shippingAddress,
                    PaymentMethod = Enum.Parse<PaymentMethod>(paymentMethod, true),
                    PaymentStatus = PaymentStatus.Pending,
                    OrderItems = new List<OrderItem>
                    {
                        new OrderItem
                        {
                            Id = Guid.NewGuid(),
                            OrderId = orderId,
                            ProductId = auction.ProductId,
                            Quantity = 1,
                            UnitPrice = auction.CurrentHighestBid
                        }
                    }
                };

                await _orderRepository.AddAsync(order);
                
                auction.OrderId = order.Id;
                auction.Status = AuctionStatus.Completed; // Already completed but ensuring consistency
                await _auctionRepository.UpdateAsync(auction);

                await transaction.CommitAsync();
                return order.Id;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
