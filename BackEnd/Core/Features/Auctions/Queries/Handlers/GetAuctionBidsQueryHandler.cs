using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Core.Features.Auctions.Queries.Models;
using Core.Features.Auctions.Queries.Results;
using Service.Services.AuctionService;
using AutoMapper;
using System.Collections.Generic;

namespace Core.Features.Auctions.Queries.Handlers
{
    public class GetAuctionBidsQueryHandler : ResponseHandler, 
        IRequestHandler<GetAuctionBidsQuery, Response<List<BidResponse>>>
    {
        private readonly IAuctionService _auctionService;
        private readonly IMapper _mapper;

        public GetAuctionBidsQueryHandler(IAuctionService auctionService, IMapper mapper)
        {
            _auctionService = auctionService;
            _mapper = mapper;
        }

        public async Task<Response<List<BidResponse>>> Handle(GetAuctionBidsQuery request, CancellationToken cancellationToken)
        {
            var bids = await _auctionService.GetBidsForAuctionAsync(request.AuctionId);
            var mapped = _mapper.Map<List<BidResponse>>(bids);
            return Success(mapped);
        }
    }
}
