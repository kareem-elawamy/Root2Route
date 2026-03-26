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
        [HttpGet("api/v1/dashboard/superadmin/overview-stats")]
        // [Authorize(Roles = "SuperAdmin")] // Uncomment when authentication matches SuperAdmin role precisely
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
        public async Task<IActionResult> ApproveOrganization([FromRoute] System.Guid id)
        {
            var response = await Mediator.Send(new Core.Features.Organization.Commands.Models.ApproveOrganizationCommand { OrganizationId = id });
            return NewResult(response);
        }

        [HttpPut("api/v1/dashboard/superadmin/organizations/{id}/reject")]
        public async Task<IActionResult> RejectOrganization([FromRoute] System.Guid id, [FromQuery] string reason)
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
    }
}
