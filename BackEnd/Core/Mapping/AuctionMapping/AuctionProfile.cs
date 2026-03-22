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
                .ForMember(dest => dest.ProductId,
                    opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.ProductName,
                    opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : string.Empty))
                .ForMember(dest => dest.HighestBidderName,
                    opt => opt.MapFrom(src => src.HighestBidder != null ? src.HighestBidder.FullName : null))
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<Bid, BidResponse>()
                .ForMember(dest => dest.BidderName,
                    opt => opt.MapFrom(src => src.Bidder != null ? src.Bidder.FullName : null));
        }
    }
}
