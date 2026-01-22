using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Base;
using Core.Features.PlantInfo.Commands.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Service.Services.PlantInfoService;

namespace Core.Features.PlantInfo.Commands.Handler
{
    public class PlantInfoCommandHandler : ResponseHandler, IRequestHandler<CreatePlantInfoCommand, Response<string>>
    {
        private readonly IPlantInfoService _plantInfoService;
        private readonly IMapper _mapper;
        public PlantInfoCommandHandler(IMapper mapper, IPlantInfoService plantInfoService)
        {
            _mapper = mapper;
            _plantInfoService = plantInfoService;
        }
        public async Task<Response<string>> Handle(CreatePlantInfoCommand request, CancellationToken cancellationToken)
        {
            var plantInfo = _mapper.Map<Domain.Models.PlantInfo>(request);
            var result = await _plantInfoService.CreatePlantInfoAsync(plantInfo, request.Image  );
            if (result == "Exists")
            {
                return BadRequest<string>("Plant info with the same name already exists.");
            }

            return Created(result);
        }
    }
}