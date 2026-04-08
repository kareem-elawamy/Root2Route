using System;
using System.Collections.Generic;
using Core.Base;
using Domain.Enums;
using MediatR;
using Service.DTOs.DashBoardDto;

namespace Core.Features.DashBoard.Queries.Models
{
    public class GetOrgOverviewStatsQuery : IRequest<Response<OrgDashboardOverviewDto>>
    {
        public Guid OrganizationId { get; set; }
    }

    public class GetOrgActivityChartQuery : IRequest<Response<IEnumerable<OrgActivityChartItemDto>>>
    {
        public Guid OrganizationId { get; set; }
        public int Months { get; set; } = 6;
    }

    public class GetOrgLiveBidsQuery : IRequest<Response<IEnumerable<LiveBidActivityDto>>>
    {
        public Guid OrganizationId { get; set; }
        public int Limit { get; set; } = 20;
    }
    public class GetOrgLatestOrdersQuery : IRequest<Response<IEnumerable<LatestOrderDto>>>
    {
        public Guid OrganizationId { get; set; }

        public int Limit { get; set; } = 10;
        public OrderStatus? StatusFilter { get; set; } = null;
    }
}
