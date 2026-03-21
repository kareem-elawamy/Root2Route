using AutoMapper;
using AutoMapper.QueryableExtensions;
using Core.Base;
using Core.Features.Orders.Queries.Models;
using Core.Features.Orders.Queries.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Service.Services.OrderService;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Features.Orders.Queries.Handlers
{
    public class GetMyOrdersQueryHandler : IRequestHandler<GetMyOrdersQuery, Response<List<OrderResponse>>>
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;

        public GetMyOrdersQueryHandler(IOrderService orderService, IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }

        public async Task<Response<List<OrderResponse>>> Handle(GetMyOrdersQuery request, CancellationToken cancellationToken)
        {
            var queryableOrders = _orderService.GetOrdersByBuyerIdQueryable(request.BuyerId);

            // 4. Generate the exact SELECT statement SQL required via ProjectTo
            var mappedOrders = await queryableOrders
                .ProjectTo<OrderResponse>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return new Response<List<OrderResponse>>(mappedOrders) { Succeeded = true };
        }
    }
}
