using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Base;
using Core.Features.DashBoard.Queries.Models;
using Domain.Enums;
using Infrastructure.Repositories.AuctionRepository;
using Infrastructure.Repositories.BidRepository;
using Infrastructure.Repositories.ChatMessageRepository;
using Infrastructure.Repositories.OrderRepository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Service.DTOs.DashBoardDto;
using Service.Services.OrgDashboardServices;

namespace Core.Features.DashBoard.Queries.Handlers
{
    public class OrgDashboardQueryHandler : ResponseHandler,
        IRequestHandler<GetOrgOverviewStatsQuery, Response<OrgDashboardOverviewDto>>,
        IRequestHandler<GetOrgActivityChartQuery, Response<IEnumerable<OrgActivityChartItemDto>>>,
        IRequestHandler<GetOrgLiveBidsQuery, Response<IEnumerable<LiveBidActivityDto>>>,
        IRequestHandler<GetOrgLatestOrdersQuery, Response<IEnumerable<LatestOrderDto>>>
    {
        private readonly IOrgDashboardServices _orgDashboardServices;
        private readonly IMapper _mapper;

        public OrgDashboardQueryHandler(
            IOrgDashboardServices orgDashboardServices,
            IMapper mapper)
        {
            _orgDashboardServices = orgDashboardServices;
            _mapper = mapper;
        }

        public async Task<Response<OrgDashboardOverviewDto>> Handle(
            GetOrgOverviewStatsQuery request,
            CancellationToken cancellationToken)
        {
            var organizationId = request.OrganizationId;
            var totalRevenue = await _orgDashboardServices.GetOverviewStatsAsync(organizationId, cancellationToken);
            return Success(totalRevenue);
        }
        public async Task<Response<IEnumerable<OrgActivityChartItemDto>>> Handle(
            GetOrgActivityChartQuery request,
            CancellationToken cancellationToken)
        {

            var organizationId = request.OrganizationId;
            var chartData = await _orgDashboardServices.GetActivityChartAsync(organizationId, request.Months, cancellationToken);
            return Success(chartData);
        }

        public async Task<Response<IEnumerable<LiveBidActivityDto>>> Handle(
            GetOrgLiveBidsQuery request,
            CancellationToken cancellationToken)
        {
            var organizationId = request.OrganizationId;
            var bids = await _orgDashboardServices.GetLiveBidsAsync(organizationId, request.Limit, cancellationToken);
            return Success(bids);
        }


        public async Task<Response<IEnumerable<LatestOrderDto>>> Handle(
            GetOrgLatestOrdersQuery request,
            CancellationToken cancellationToken)
        {
            var organizationId = request.OrganizationId;
            var orders = await _orgDashboardServices.GetLatestOrdersAsync(organizationId, request.Limit, request.StatusFilter, cancellationToken);
            return Success(orders);
        }
    }
}
