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
    public class GetActiveAuctionsQueryHandler : ResponseHandler, 
        IRequestHandler<GetActiveAuctionsQuery, Response<List<AuctionResponse>>>
    {
        private readonly IAuctionService _auctionService;
        private readonly IMapper _mapper;

        public GetActiveAuctionsQueryHandler(IAuctionService auctionService, IMapper mapper)
        {
            _auctionService = auctionService;
            _mapper = mapper;
        }

        public async Task<Response<List<AuctionResponse>>> Handle(GetActiveAuctionsQuery request, CancellationToken cancellationToken)
        {
            var filter = new AuctionFilter
            {
                SearchTerm = request.SearchTerm,
                MinPrice = request.MinPrice,
                MaxPrice = request.MaxPrice,
                CategoryId = request.CategoryId,
                SortBy = request.SortBy
            };
            var auctions = await _auctionService.GetActiveAuctionsAsync(filter, request.PageNumber, request.PageSize);
            var mapped = _mapper.Map<List<AuctionResponse>>(auctions);
            return Success(mapped);
        }
    }
}
