using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Base;
using Core.Features.PlantInfo.Queries.Models;
using Core.Features.PlantInfo.Queries.Result;
using MediatR;
using Service.Services.PlantInfoService;

namespace Core.Features.PlantInfo.Queries.Handler
{
    public class PalntInfoHandler : ResponseHandler, IRequestHandler<GetAllPlantInfosQuery, Response<List<PlantInfoListResponse>>>
    {
        private readonly IPlantInfoService _plantInfoService;
        private readonly IMapper _mapper;
        public PalntInfoHandler(IPlantInfoService plantInfoService, IMapper mapper)
        {
            _plantInfoService = plantInfoService;
            _mapper = mapper;
        }
        public async Task<Response<List<PlantInfoListResponse>>> Handle(GetAllPlantInfosQuery request, CancellationToken cancellationToken)
        {
            var plantInfos = await _plantInfoService.GetAllPlantInfosAsync();
            var mappedPlantInfos = _mapper.Map<List<PlantInfoListResponse>>(plantInfos);
            return Success(mappedPlantInfos);
        }
    }
}