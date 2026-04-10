using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enums;
using Service.DTOs.DashBoardDto;

namespace Service.Services.OrgDashboardServices
{
    public interface IOrgDashboardServices
    {
        public Task<OrgDashboardOverviewDto> GetOverviewStatsAsync(Guid organizationId, CancellationToken cancellationToken = default);
        public Task<IEnumerable<OrgActivityChartItemDto>> GetActivityChartAsync(Guid organizationId,
            int months = 6, CancellationToken cancellationToken = default);
        public Task<IEnumerable<LiveBidActivityDto>> GetLiveBidsAsync(Guid organizationId,
            int limit = 20, CancellationToken cancellationToken = default);
        public Task<IEnumerable<LatestOrderDto>> GetLatestOrdersAsync(Guid organizationId,
            int limit = 10, OrderStatus? statusFilter = null, CancellationToken cancellationToken = default);
    }
}