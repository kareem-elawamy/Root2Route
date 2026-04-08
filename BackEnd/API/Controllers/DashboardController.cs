using System;
using System.Threading.Tasks;
using API.Controllers.Shared;
using Core.Features.DashBoard.Queries.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    public class DashboardController : BaseApiController
    {
        // ════════════════════════════════════════════════════════════════════════
        // SuperAdmin endpoints (existing)
        // ════════════════════════════════════════════════════════════════════════

        [HttpGet("api/v1/dashboard/superadmin/overview-stats")]
        // [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> GetOverviewStats()
        {
            var response = await Mediator.Send(new GetOverviewStatsModel());
            return NewResult(response);
        }

        [HttpGet("api/v1/dashboard/superadmin/financials")]
        public async Task<IActionResult> GetFinancials([FromQuery] int months = 6)
        {
            var response = await Mediator.Send(new GetFinancialsModel { Months = months });
            return NewResult(response);
        }

        [HttpGet("api/v1/dashboard/superadmin/ml/top-diseases")]
        public async Task<IActionResult> GetTopDiseases([FromQuery] int top = 5)
        {
            var response = await Mediator.Send(new GetTopDiseasesModel { Top = top });
            return NewResult(response);
        }

        [HttpGet("api/v1/dashboard/superadmin/ml/accuracy-trend")]
        public async Task<IActionResult> GetMLAccuracyTrend([FromQuery] int days = 14)
        {
            var response = await Mediator.Send(new GetMLAccuracyTrendModel { Days = days });
            return NewResult(response);
        }

        [HttpGet("api/v1/dashboard/superadmin/organizations/pending")]
        public async Task<IActionResult> GetPendingOrganizations()
        {
            var response = await Mediator.Send(new GetPendingOrganizationsModel());
            return NewResult(response);
        }

        [HttpGet("api/v1/dashboard/superadmin/ml/disease-heatmap")]
        public async Task<IActionResult> GetDiseaseHeatmap()
        {
            var response = await Mediator.Send(new GetDiseaseHeatmapQuery());
            return NewResult(response);
        }

        [HttpPut("api/v1/dashboard/superadmin/organizations/{id}/approve")]
        public async Task<IActionResult> ApproveOrganization([FromRoute] Guid id)
        {
            var response = await Mediator.Send(new Core.Features.Organization.Commands.Models.ApproveOrganizationCommand { OrganizationId = id });
            return NewResult(response);
        }

        [HttpPut("api/v1/dashboard/superadmin/organizations/{id}/reject")]
        public async Task<IActionResult> RejectOrganization([FromRoute] Guid id, [FromQuery] string reason)
        {
            var response = await Mediator.Send(new Core.Features.Organization.Commands.Models.RejectOrganizationCommand { OrganizationId = id, Reason = reason });
            return NewResult(response);
        }

        [HttpPut("api/v1/dashboard/superadmin/settings/platform-fee")]
        public async Task<IActionResult> UpdatePlatformFee([FromQuery] decimal newFee)
        {
            var response = await Mediator.Send(new Core.Features.SystemSettings.Commands.Models.UpdatePlatformFeeCommand { NewFeePercentage = newFee });
            return NewResult(response);
        }

        // ════════════════════════════════════════════════════════════════════════
        // Organisation Dashboard – Overview page
        // Route prefix: api/v1/dashboard/org/{orgId}/
        // ════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Returns the four KPI summary cards:
        ///   Total Revenue | Active Auctions | Pending Orders | Unread Messages
        /// </summary>
        [HttpGet("api/v1/dashboard/org/{orgId}/overview")]
        public async Task<IActionResult> GetOrgOverview([FromRoute] Guid orgId)
        {
            var response = await Mediator.Send(new GetOrgOverviewStatsQuery { OrganizationId = orgId });
            return NewResult(response);
        }

        /// <summary>
        /// Returns monthly Net Revenue + Auction Volume for the Activity Over Time chart.
        /// </summary>
        [HttpGet("api/v1/dashboard/org/{orgId}/activity-chart")]
        public async Task<IActionResult> GetOrgActivityChart(
            [FromRoute] Guid orgId,
            [FromQuery] int months = 6)
        {
            var response = await Mediator.Send(new GetOrgActivityChartQuery
            {
                OrganizationId = orgId,
                Months         = months
            });
            return NewResult(response);
        }

        /// <summary>
        /// Returns the most-recent bids placed on this organisation's auctions (Live Bid Activity feed).
        /// </summary>
        [HttpGet("api/v1/dashboard/org/{orgId}/live-bids")]
        public async Task<IActionResult> GetOrgLiveBids(
            [FromRoute] Guid orgId,
            [FromQuery] int limit = 20)
        {
            var response = await Mediator.Send(new GetOrgLiveBidsQuery
            {
                OrganizationId = orgId,
                Limit          = limit
            });
            return NewResult(response);
        }

        /// <summary>
        /// Returns the latest orders placed with this organisation (Latest Orders table).
        /// </summary>
        [HttpGet("api/v1/dashboard/org/{orgId}/latest-orders")]
        public async Task<IActionResult> GetOrgLatestOrders(
            [FromRoute] Guid orgId,
            [FromQuery] int limit = 10)
        {
            var response = await Mediator.Send(new GetOrgLatestOrdersQuery
            {
                OrganizationId = orgId,
                Limit          = limit
            });
            return NewResult(response);
        }
    }
}

