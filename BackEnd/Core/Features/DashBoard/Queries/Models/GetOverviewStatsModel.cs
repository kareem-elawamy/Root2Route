using System.Collections.Generic;
using Core.Base;
using MediatR;
using Service.DTOs;

namespace Core.Features.DashBoard.Queries.Models
{
    public class GetOverviewStatsModel : IRequest<Response<DashboardOverviewDto>>
    {
    }

    public class GetFinancialsModel : IRequest<Response<IEnumerable<FinancialChartItemDto>>>
    {
        public int Months { get; set; } = 6;
    }

    public class GetTopDiseasesModel : IRequest<Response<IEnumerable<Service.DTOs.DashBoardDto.DiseaseInsightDto>>>
    {
        public int Top { get; set; } = 5;
    }

    public class GetPendingOrganizationsModel : IRequest<Response<IEnumerable<Service.DTOs.DashBoardDto.PendingOrganizationDto>>>
    {
    }

    public class GetMLAccuracyTrendModel : IRequest<Response<IEnumerable<Service.DTOs.DashBoardDto.MLAccuracyTrendDto>>>
    {
        public int Days { get; set; } = 14;
    }
}