using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Core.Base;
using Core.Features.DashBoard.Queries.Models;
using Domain.Enums;
using Infrastructure.Repositories.DiagnosisLogRepository;
using Infrastructure.Repositories.OrderRepository;
using Infrastructure.Repositories.OrganizationRepository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Service.DTOs;
using Service.DTOs.DashBoardDto;

namespace Core.Features.DashBoard.Queries.Handlers
{
    public class DashboardQueryHandler : ResponseHandler,
        IRequestHandler<GetOverviewStatsModel, Response<DashboardOverviewDto>>,
        IRequestHandler<GetFinancialsModel, Response<IEnumerable<FinancialChartItemDto>>>,
        IRequestHandler<GetTopDiseasesModel, Response<IEnumerable<DiseaseInsightDto>>>,
        IRequestHandler<GetPendingOrganizationsModel, Response<IEnumerable<PendingOrganizationDto>>>,
        IRequestHandler<GetMLAccuracyTrendModel, Response<IEnumerable<MLAccuracyTrendDto>>>,
        IRequestHandler<GetDiseaseHeatmapQuery, Response<IEnumerable<DiseaseHeatmapDto>>>
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IOrganizationRepository _orgRepo;
        private readonly IDiagnosisLogRepository _diagnosisRepo;
        private readonly IMapper _mapper;

        public DashboardQueryHandler(
            IOrderRepository orderRepo,
            IOrganizationRepository orgRepo,
            IDiagnosisLogRepository diagnosisRepo,
            IMapper mapper)
        {
            _orderRepo = orderRepo;
            _orgRepo = orgRepo;
            _diagnosisRepo = diagnosisRepo;
            _mapper = mapper;
        }

        public async Task<Response<DashboardOverviewDto>> Handle(GetOverviewStatsModel request, CancellationToken cancellationToken)
        {
            var grossRevenue = await _orderRepo.GetTableNoTracking().SumAsync(o => o.TotalAmount, cancellationToken);
            var platformFees = await _orderRepo.GetTableNoTracking().SumAsync(o => o.PlatformFee, cancellationToken);
            var pendingOrgs = await _orgRepo.GetTableNoTracking().CountAsync(o => o.OrganizationStatus == OrganizationStatus.Pending, cancellationToken);
            var mlDiagnoses = await _diagnosisRepo.GetTableNoTracking().CountAsync(cancellationToken);

            var dto = new DashboardOverviewDto
            {
                GrossRevenue = grossRevenue,
                PlatformFees = platformFees,
                PendingOrganizations = pendingOrgs,
                TotalMLDiagnoses = mlDiagnoses
            };

            return Success(dto);
        }

        public async Task<Response<IEnumerable<FinancialChartItemDto>>> Handle(GetFinancialsModel request, CancellationToken cancellationToken)
        {
            var startDate = DateTime.UtcNow.AddMonths(-request.Months);

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
                .ToListAsync(cancellationToken);

            var result = orders.Select(o => new FinancialChartItemDto
            {
                Month = new DateTime(o.Year, o.Month, 1).ToString("MMM yyyy"),
                Revenue = o.Revenue,
                Fees = o.Fees
            });

            return Success(result);
        }

        public async Task<Response<IEnumerable<DiseaseInsightDto>>> Handle(GetTopDiseasesModel request, CancellationToken cancellationToken)
        {
            var logs = await _diagnosisRepo.GetTableNoTracking()
                .Where(d => !d.Prediction.Contains("Healthy"))
                .GroupBy(d => d.Prediction)
                .Select(g => new
                {
                    DiseaseName = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .Take(request.Top)
                .ToListAsync(cancellationToken);

            var result = logs.Select(l => new DiseaseInsightDto
            {
                DiseaseName = l.DiseaseName,
                Count = l.Count
            });

            return Success(result);
        }

        public async Task<Response<IEnumerable<PendingOrganizationDto>>> Handle(GetPendingOrganizationsModel request, CancellationToken cancellationToken)
        {
            var orgs = await _orgRepo.GetTableNoTracking()
                .Where(o => o.OrganizationStatus == OrganizationStatus.Pending)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync(cancellationToken);

            var mapped = _mapper.Map<IEnumerable<PendingOrganizationDto>>(orgs);
            return Success(mapped);
        }

        public async Task<Response<IEnumerable<MLAccuracyTrendDto>>> Handle(GetMLAccuracyTrendModel request, CancellationToken cancellationToken)
        {
            var startDate = DateTime.UtcNow.AddDays(-request.Days);

            var logs = await _diagnosisRepo.GetTableNoTracking()
                .Where(d => d.CreatedAt >= startDate)
                .GroupBy(d => d.CreatedAt.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    AvgConfidence = g.Average(x => x.Confidence)
                })
                .OrderBy(x => x.Date)
                .ToListAsync(cancellationToken);

            var result = logs.Select(l => new MLAccuracyTrendDto
            {
                DateLabel = l.Date.ToString("dd MMM"),
                AverageConfidence = Math.Round(l.AvgConfidence * 100, 2)
            });

            return Success(result);
        }

        public async Task<Response<IEnumerable<DiseaseHeatmapDto>>> Handle(GetDiseaseHeatmapQuery request, CancellationToken cancellationToken)
        {
            var logs = await _diagnosisRepo.GetTableNoTracking()
                .Where(d => d.Latitude.HasValue && d.Longitude.HasValue)
                .GroupBy(d => new { d.Prediction, d.Latitude, d.Longitude, d.City, d.Region })
                .Select(g => new DiseaseHeatmapDto
                {
                    DiseaseName = g.Key.Prediction,
                    Latitude = g.Key.Latitude.Value,
                    Longitude = g.Key.Longitude.Value,
                    City = g.Key.City,
                    Region = g.Key.Region,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .ToListAsync(cancellationToken);

            return Success<IEnumerable<DiseaseHeatmapDto>>(logs);
        }
    }
}
