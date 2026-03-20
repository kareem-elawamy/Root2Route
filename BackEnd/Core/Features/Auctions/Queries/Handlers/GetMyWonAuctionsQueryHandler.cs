using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using Core.Features.Auctions.Queries.Models;
using Core.Features.Auctions.Queries.Results;
using Service.Services.AuctionService;

namespace Core.Features.Auctions.Queries.Handlers
{
    public class GetMyWonAuctionsQueryHandler : ResponseHandler,
        IRequestHandler<GetMyWonAuctionsQuery, Response<List<AuctionResponse>>>
    {
        private readonly IAuctionService _auctionService;
        private readonly IMapper _mapper;

        public GetMyWonAuctionsQueryHandler(IAuctionService auctionService, IMapper mapper)
        {
            _auctionService = auctionService;
            _mapper = mapper;
        }

        public async Task<Response<List<AuctionResponse>>> Handle(GetMyWonAuctionsQuery request, CancellationToken cancellationToken)
        {
            var auctions = await _auctionService.GetMyWonAuctionsAsync(request.UserId);
            return Success(_mapper.Map<List<AuctionResponse>>(auctions));
        }
    }
}
