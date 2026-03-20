using MediatR;
using System;
using System.Collections.Generic;
using Core.Features.Auctions.Queries.Results;

namespace Core.Features.Auctions.Queries.Models
{
    public class GetMyParticipatedAuctionsQuery : IRequest<Response<List<AuctionResponse>>>
    {
        public Guid UserId { get; set; }
        public GetMyParticipatedAuctionsQuery(Guid userId) => UserId = userId;
    }
}
