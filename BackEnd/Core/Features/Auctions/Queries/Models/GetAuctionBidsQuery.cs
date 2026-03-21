using MediatR;
using System;
using System.Collections.Generic;
using Core.Features.Auctions.Queries.Results;

namespace Core.Features.Auctions.Queries.Models
{
    public class GetAuctionBidsQuery : IRequest<Response<List<BidResponse>>>
    {
        public Guid AuctionId { get; set; }
        public GetAuctionBidsQuery(Guid auctionId) => AuctionId = auctionId;
    }
}
