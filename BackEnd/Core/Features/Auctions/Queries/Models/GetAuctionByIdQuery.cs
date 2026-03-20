using MediatR;
using System;
using Core.Features.Auctions.Queries.Results;

namespace Core.Features.Auctions.Queries.Models
{
    public class GetAuctionByIdQuery : IRequest<Response<AuctionResponse>>
    {
        public Guid Id { get; set; }
        public GetAuctionByIdQuery(Guid id) => Id = id;
    }
}
