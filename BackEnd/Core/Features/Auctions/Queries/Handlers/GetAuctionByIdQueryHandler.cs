using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Core.Features.Auctions.Queries.Models;
using Core.Features.Auctions.Queries.Results;
using Service.Services.AuctionService;
using AutoMapper;

namespace Core.Features.Auctions.Queries.Handlers
{
    public class GetAuctionByIdQueryHandler : ResponseHandler, 
        IRequestHandler<GetAuctionByIdQuery, Response<AuctionResponse>>
    {
        private readonly IAuctionService _auctionService;
        private readonly IMapper _mapper;

        public GetAuctionByIdQueryHandler(IAuctionService auctionService, IMapper mapper)
        {
            _auctionService = auctionService;
            _mapper = mapper;
        }

        public async Task<Response<AuctionResponse>> Handle(GetAuctionByIdQuery request, CancellationToken cancellationToken)
        {
            var auction = await _auctionService.GetAuctionByIdAsync(request.Id);
            if (auction == null) return NotFound<AuctionResponse>("Auction not found");
            var mapped = _mapper.Map<AuctionResponse>(auction);
            return Success(mapped);
        }
    }
}
