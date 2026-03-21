using AutoMapper;
using Core.Features.Auctions.Queries.Results;
using Domain.Models;

namespace Core.Mapping.AuctionMapping
{
    public class AuctionProfile : Profile
    {
        public AuctionProfile()
        {
            CreateMap<Auction, AuctionResponse>()
                // productid is lowercase in the Domain entity — explicit mapping required
                .ForMember(dest => dest.ProductId,
                    opt => opt.MapFrom(src => src.productid))
                // Flatten Product.Name safely (null-safe)
                .ForMember(dest => dest.ProductName,
                    opt => opt.MapFrom(src => src.product != null ? src.product.Name : string.Empty))
                // Flatten HighestBidder.FullName safely (null-safe)
                .ForMember(dest => dest.HighestBidderName,
                    opt => opt.MapFrom(src => src.HighestBidder != null ? src.HighestBidder.FullName : null))
                // Convert AuctionStatus enum to its string name
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<Bid, BidResponse>()
                // Flatten Bidder.FullName safely (null-safe)
                .ForMember(dest => dest.BidderName,
                    opt => opt.MapFrom(src => src.Bidder != null ? src.Bidder.FullName : null));
        }
    }
}
