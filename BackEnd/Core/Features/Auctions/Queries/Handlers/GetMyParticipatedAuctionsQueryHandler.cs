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
    public class GetMyParticipatedAuctionsQueryHandler : ResponseHandler,
        IRequestHandler<GetMyParticipatedAuctionsQuery, Response<List<AuctionResponse>>>
    {
        private readonly IAuctionService _auctionService;
        private readonly IMapper _mapper;

        public GetMyParticipatedAuctionsQueryHandler(IAuctionService auctionService, IMapper mapper)
        {
            _auctionService = auctionService;
            _mapper = mapper;
        }

        public async Task<Response<List<AuctionResponse>>> Handle(GetMyParticipatedAuctionsQuery request, CancellationToken cancellationToken)
        {
            var auctions = await _auctionService.GetMyParticipatedAuctionsAsync(request.UserId);
            return Success(_mapper.Map<List<AuctionResponse>>(auctions));
        }
    }
}
