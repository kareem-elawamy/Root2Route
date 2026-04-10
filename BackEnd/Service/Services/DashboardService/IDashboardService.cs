
using Service.DTOs;
using Service.DTOs.DashBoardDto;

namespace Service.Services.DashboardService
{
    public interface IDashboardService
    {
        Task<DashboardOverviewDto> GetOverviewStatsAsync();
        Task<IEnumerable<FinancialChartItemDto>> GetFinancialOverviewAsync(int months = 6);
        Task<IEnumerable<DiseaseInsightDto>> GetTopDiseasesAsync(int top = 5);
        Task<IEnumerable<PendingOrganizationDto>> GetPendingOrganizationsAsync();
        Task<IEnumerable<MLAccuracyTrendDto>> GetMLAccuracyTrendAsync(int days = 14);

    }
}