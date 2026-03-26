using System.Collections.Generic;
using Core.Base;
using MediatR;
using Service.DTOs.DashBoardDto;

namespace Core.Features.DashBoard.Queries.Models
{
    public class GetDiseaseHeatmapQuery : IRequest<Response<IEnumerable<DiseaseHeatmapDto>>>
    {
    }
}
