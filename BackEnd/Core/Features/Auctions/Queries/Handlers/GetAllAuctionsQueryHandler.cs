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
    public class GetAllAuctionsQueryHandler : ResponseHandler, 
        IRequestHandler<GetAllAuctionsQuery, Response<List<AuctionResponse>>>
    {
        private readonly IAuctionService _auctionService;
        private readonly IMapper _mapper;

        public GetAllAuctionsQueryHandler(IAuctionService auctionService, IMapper mapper)
        {
            _auctionService = auctionService;
            _mapper = mapper;
        }

        public async Task<Response<List<AuctionResponse>>> Handle(GetAllAuctionsQuery request, CancellationToken cancellationToken)
        {
            var filter = new AuctionFilter
            {
                SearchTerm = request.SearchTerm,
                MinPrice = request.MinPrice,
                MaxPrice = request.MaxPrice,
                SortBy = request.SortBy
            };
            var auctions = await _auctionService.GetAllAuctionsAsync(filter, request.PageNumber, request.PageSize);
            var mapped = _mapper.Map<List<AuctionResponse>>(auctions);
            return Success(mapped);
        }
    }
}
