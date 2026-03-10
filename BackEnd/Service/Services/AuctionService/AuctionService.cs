using System;
using System.Threading.Tasks;
using Domain.Enums;
using Domain.Models; // مهم جداً عشان كيانات Auction و Bid
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

        // التعديل هنا: اتغير الاسم لـ productId بدل marketItemId
        public async Task<string> CreateAuctionAsync(Guid productId, int durationInHours)
        {
            // 1. جلب المنتج
            var item = await _productRepository.GetByIdAsync(productId);

            // 2. التحقق من وجوده وصلاحيته للمزاد
            if (item == null || !item.IsAvailableForAuction)
                return "Product not found or not available for auction";

            // 3. إنشاء المزاد
            var auction = new Auction
            {
                // إحنا مش محتاجين ندي Id بـ Guid.NewGuid() لأن EF بيكريته لوحده 
                // بس لو عامل الكونستركتور في BaseEntity بيكريته يبقى مفيش مشكلة

                productid = productId, // التعديل الأهم: ProductId بدل MarketItemId
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
            // 1. جلب المزاد
            var auction = await _auctionRepository.GetByIdAsync(auctionId);
            if (auction == null) return "Auction not found";

            // 2. التحقق من حالة المزاد
            if (auction.Status != AuctionStatus.Ongoing) return "Auction is not ongoing"; // أمان أكتر
            if (auction.EndDate < DateTime.UtcNow) return "Auction has expired";

            // 3. التحقق من مبلغ المزايدة
            if (amount <= auction.CurrentHighestBid)
                return "Bid must be higher than the current highest bid";

            // 4. إنشاء المزايدة
            var newBid = new Bid
            {
                AuctionId = auctionId,
                BidderId = bidderId,
                Amount = amount,
                BidTime = DateTime.UtcNow
            };

            // 5. حفظ المزايدة وتحديث سعر المزاد الأعلى
            await _bidRepository.AddAsync(newBid);

            auction.CurrentHighestBid = amount;
            auction.HighestBidderId = bidderId; // 👈 إضافة مهمة جداً: لازم نسجل مين أعلى مزايد حالياً

            await _auctionRepository.UpdateAsync(auction);

            return "Bid Placed Successfully";
        }
    }
}