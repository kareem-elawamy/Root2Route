using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Service.DTOs;
using Domain.Enums;
using Infrastructure.Repositories.OrderRepository;
using Infrastructure.Repositories.OrganizationRepository;
using Infrastructure.Repositories.DiagnosisLogRepository;
using Service.DTOs.DashBoardDto;

namespace Service.Services.DashboardService
{
    public class DashboardService : IDashboardService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IOrganizationRepository _orgRepo;
        private readonly IDiagnosisLogRepository _diagnosisRepo;

        public DashboardService(
            IOrderRepository orderRepo,
            IOrganizationRepository orgRepo,
            IDiagnosisLogRepository diagnosisRepo)
        {
            _orderRepo = orderRepo;
            _orgRepo = orgRepo;
            _diagnosisRepo = diagnosisRepo;
        }

        public async Task<DashboardOverviewDto> GetOverviewStatsAsync()
        {
            var grossRevenue = await _orderRepo.GetTableNoTracking().SumAsync(o => o.TotalAmount);
            var platformFees = await _orderRepo.GetTableNoTracking().SumAsync(o => o.PlatformFee);
            var pendingOrgs = await _orgRepo.GetTableNoTracking().CountAsync(o => o.OrganizationStatus == OrganizationStatus.Pending);
            
            var mlDiagnoses = await _diagnosisRepo.GetTableNoTracking().CountAsync();

            return new DashboardOverviewDto
            {
                GrossRevenue = grossRevenue,
                PlatformFees = platformFees,
                PendingOrganizations = pendingOrgs,
                TotalMLDiagnoses = mlDiagnoses
            };
        }

        public async Task<IEnumerable<FinancialChartItemDto>> GetFinancialOverviewAsync(int months = 6)
        {
            var startDate = DateTime.UtcNow.AddMonths(-months);

            var orders = await _orderRepo.GetTableNoTracking()
                .Where(o => o.OrderDate >= startDate)
                .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Revenue = g.Sum(o => o.TotalAmount),
                    Fees = g.Sum(o => o.PlatformFee)
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync();

            return orders.Select(o => new FinancialChartItemDto
            {
                Month = new DateTime(o.Year, o.Month, 1).ToString("MMM yyyy"),
                Revenue = o.Revenue,
                Fees = o.Fees
            });
        }

        public async Task<IEnumerable<DiseaseInsightDto>> GetTopDiseasesAsync(int top = 5)
        {
            var logs = await _diagnosisRepo.GetTableNoTracking()
                .GroupBy(d => d.Prediction)
                .Select(g => new
                {
                    DiseaseName = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .Take(top)
                .ToListAsync();

            return logs.Select(l => new DiseaseInsightDto
            {
                DiseaseName = l.DiseaseName,
                Count = l.Count
            });
        }

        public async Task<IEnumerable<PendingOrganizationDto>> GetPendingOrganizationsAsync()
        {
            var orgs = await _orgRepo.GetTableNoTracking()
                .Where(o => o.OrganizationStatus == OrganizationStatus.Pending)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new
                {
                    Id = o.Id,
                    Name = o.Name,
                    Type = o.Type,
                    ContactEmail = o.ContactEmail,
                    RequestDate = o.CreatedAt
                })
                .ToListAsync();

            return orgs.Select(o => new PendingOrganizationDto
            {
                Id = o.Id,
                Name = o.Name,
                Type = o.Type.ToString(),
                ContactEmail = o.ContactEmail,
                RequestDate = o.RequestDate
            });
        }

        public async Task<IEnumerable<MLAccuracyTrendDto>> GetMLAccuracyTrendAsync(int days = 14)
        {
            var startDate = DateTime.UtcNow.AddDays(-days);

            var logs = await _diagnosisRepo.GetTableNoTracking()
                .Where(d => d.CreatedAt >= startDate)
                .GroupBy(d => d.CreatedAt.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    AvgConfidence = g.Average(x => x.Confidence)
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

            return logs.Select(l => new MLAccuracyTrendDto
            {
                DateLabel = l.Date.ToString("dd MMM"),
                AverageConfidence = Math.Round(l.AvgConfidence * 100, 2)
            });
        }



    }
}
