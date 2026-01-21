using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Base;
using Core.Features.PlantInfo.Queries.Result;
using MediatR;

namespace Core.Features.PlantInfo.Queries.Models
{
    public class GetAllPlantInfosQuery : IRequest<Response<List<PlantInfoListResponse>>>
    {
        
    }
}