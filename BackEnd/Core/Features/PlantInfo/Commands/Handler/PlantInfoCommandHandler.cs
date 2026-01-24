using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Base;
using Core.Features.PlantInfo.Commands.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Service.Services.PlantInfoService;

namespace Core.Features.PlantInfo.Commands.Handler
{
    public class PlantInfoCommandHandler : ResponseHandler, IRequestHandler<CreatePlantInfoCommand, Response<string>>,
    IRequestHandler<EditPlantInfoCommand, Response<string>>,
    IRequestHandler<DeletePlantInfoCommand, Response<string>>
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
            var result = await _plantInfoService.CreatePlantInfoAsync(plantInfo, request.Image);
            if (result == "Exists")
            {
                return BadRequest<string>("Plant info with the same name already exists.");
            }

            return Created(result);
        }

        public async Task<Response<string>> Handle(EditPlantInfoCommand request, CancellationToken cancellationToken)
        {
            // Get Plant 
            var plantInfo = await _plantInfoService.GetPlantInfoByIdAsync(request.Id);
            if (plantInfo == null) return NotFound<string>("Plant Not Found");
            var mapperPlantInfo = _mapper.Map(request, plantInfo);
            var result = await _plantInfoService.EditPlantInfoAsync(mapperPlantInfo);
            if (result == "Success") return Success<string>("Edit Success");
            else return BadRequest<string>();

        }

        public async Task<Response<string>> Handle(DeletePlantInfoCommand request, CancellationToken cancellationToken)
        {
            var plantInfo = await _plantInfoService.GetPlantInfoByIdAsync(request.Id);
            if (plantInfo == null) return NotFound<string>("Plant Not Found");
            var result = await _plantInfoService.DeletePlantInfoAsync(plantInfo);
             if (result=="Success") return Deleted<string>();
            else return BadRequest<string>();
            
        }
    }
}