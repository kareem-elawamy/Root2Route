using MediatR;
using System.Collections.Generic;
using Core.Features.Auctions.Queries.Results;

namespace Core.Features.Auctions.Queries.Models
{
    public class GetActiveAuctionsQuery : IRequest<Response<List<AuctionResponse>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
