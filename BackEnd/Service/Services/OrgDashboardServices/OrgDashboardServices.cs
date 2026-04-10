using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enums;
using Infrastructure.Repositories.AuctionRepository;
using Infrastructure.Repositories.ChatMessageRepository;
using Infrastructure.Repositories.OrderRepository;
using Infrastructure.Repositories.OrganizationRepository;
using Microsoft.EntityFrameworkCore;
using Service.DTOs.DashBoardDto;

namespace Service.Services.OrgDashboardServices
{
    public class OrgDashboardServices : IOrgDashboardServices
    {
        private readonly IOrganizationRepository _orgRepo;
        private readonly IOrderRepository _orderRepo;
        private readonly IAuctionRepository _auctionRepo;
        private readonly IChatMessageRepository _chatMessageRepo;
        public OrgDashboardServices(IOrganizationRepository orgRepo, IOrderRepository orderRepo, IAuctionRepository auctionRepo, IChatMessageRepository chatMessageRepo)
        {
            _orderRepo = orderRepo;
            _auctionRepo = auctionRepo;
            _chatMessageRepo = chatMessageRepo;
            _orgRepo = orgRepo;
        }
        //Activity Chart 
        public async Task<IEnumerable<OrgActivityChartItemDto>> GetActivityChartAsync(Guid organizationId, int months = 6, CancellationToken cancellationToken = default)
        {
            // ابد من تاريخ اليوم ناقص عدد الشهور المطلوبة
            var startDate = DateTime.UtcNow.AddMonths(-months);
            //اجيب الاردات اللي جت من بداية الفترة لليوم
            var revenueByMonth = await _orderRepo
                .GetTableNoTracking()
                .Where(o => o.OrganizationId == organizationId && o.OrderDate >= startDate)
                .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    NetRevenue = g.Sum(o => o.TotalAmount)
                }).ToListAsync(cancellationToken);
            //اجيب المزادات اللي انتهت من بداية الفترة لليوم
            var auctionByMonth = await _auctionRepo
                .GetTableNoTracking()
                .Where(a => a.Product != null &&
                            a.Product.OrganizationId == organizationId &&
                            a.EndDate >= startDate)
                .GroupBy(a => new { a.EndDate.Year, a.EndDate.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    AuctionVolume = g.Count()
                })
                .ToListAsync(cancellationToken);
            // أبني قائمة بالشهور المطلوبة، وأدمج بيانات الإيرادات والمزادات فيها
            var month = Enumerable
            .Range(0, months)
            .Select(i => DateTime.UtcNow.AddMonths(-i))
            .Select(d => new { d.Year, d.Month })
            .OrderBy(d => d.Year).ThenBy(d => d.Month)
            .ToList();
            // ادمج البيانات في DTO واحد لكل شهر
            var result = month.Select(m =>
           {
               var rev = revenueByMonth.FirstOrDefault(r => r.Year == m.Year && r.Month == m.Month);
               var auc = auctionByMonth.FirstOrDefault(a => a.Year == m.Year && a.Month == m.Month);

               return new OrgActivityChartItemDto
               {
                   Month = new DateTime(m.Year, m.Month, 1).ToString("MMM yyyy"),
                   NetRevenue = rev?.NetRevenue ?? 0m,
                   AuctionVolume = auc?.AuctionVolume ?? 0
               };
           });

            return result;
        }

        public async Task<IEnumerable<LatestOrderDto>> GetLatestOrdersAsync(Guid organizationId, int limit = 10, OrderStatus? statusFilter = null, CancellationToken cancellationToken = default)
        {
            var org = await _orgRepo.GetByIdAsync(organizationId);
            if (org == null)
                throw new Exception("Organization not found");
            var orders = await _orderRepo.GetTableNoTracking()
                        .Where(o => o.OrganizationId == organizationId && (statusFilter == null || o.Status == statusFilter))
                        .OrderByDescending(o => o.OrderDate)
                        .Take(limit)
                        .Select(o => new LatestOrderDto
                        {
                            OrderId = o.Id,
                            OrderDate = o.OrderDate,
                            TotalAmount = o.TotalAmount,
                            Status = o.Status,
                            BuyerName = o.Buyer != null ? (o.Buyer.UserName ?? o.Buyer.Email ?? "Unknown") : "Unknown"
                        }).ToListAsync(cancellationToken);
            return orders;
        }

        public async Task<IEnumerable<LiveBidActivityDto>> GetLiveBidsAsync(Guid organizationId, int limit = 20, CancellationToken cancellationToken = default)
        {
            var bids = await _auctionRepo
                .GetTableNoTracking()
                .Where(a => a.Product != null &&
                            a.Product.OrganizationId == organizationId)
                .SelectMany(a => a.Bids.Select(b => new LiveBidActivityDto
                {
                    BidId = b.Id,
                    AuctionTitle = a.Title,
                    BidderName = b.Bidder != null
                                    ? (b.Bidder.UserName ?? b.Bidder.Email ?? "Unknown")
                                    : "Unknown",
                    Amount = b.Amount,
                    BidTime = b.BidTime
                }))
                .OrderByDescending(b => b.BidTime)
                .Take(limit)
                .ToListAsync(cancellationToken);
            return bids;
        }

        public async Task<OrgDashboardOverviewDto> GetOverviewStatsAsync(Guid organizationId, CancellationToken cancellationToken = default)
        {

            var totalRevenue = await _orderRepo
                .GetTableNoTracking()
                .Where(o => o.OrganizationId == organizationId && o.Status == OrderStatus.Delivered)
                .SumAsync(o => o.TotalAmount, cancellationToken);
            var activeAuctions = await _auctionRepo
               .GetTableNoTracking()
               .Where(a => a.Product != null &&
                           a.Product.OrganizationId == organizationId &&
                           (a.Status == AuctionStatus.Upcoming || a.Status == AuctionStatus.Ongoing))
               .CountAsync(cancellationToken);
            var pendingOrders = await _orderRepo
               .GetTableNoTracking()
               .Where(o => o.OrganizationId == organizationId &&
                           (o.Status == OrderStatus.Pending || o.Status == OrderStatus.Processing))
               .CountAsync(cancellationToken);
            var unreadMessages = await _chatMessageRepo
                            .GetTableNoTracking()
                            .Where(m => m.ChatRoom != null &&
                                        m.ChatRoom.OrganizationId == organizationId &&
                                        !m.IsRead)
                            .CountAsync(cancellationToken);
            var dto = new OrgDashboardOverviewDto
            {
                TotalRevenue = totalRevenue,
                ActiveAuctions = activeAuctions,
                PendingOrders = pendingOrders,
                UnreadMessages = unreadMessages
            };
            return dto;
        }
    }
}