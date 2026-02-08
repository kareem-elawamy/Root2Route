using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Base;
using Core.Features.Farm.Commands.Models;
using Domain.Enums;
using MediatR;
using Service.Services.FarmService;

namespace Core.Features.Farm.Commands.Handler
{
    public class FarmHandlerCommand : ResponseHandler, IRequestHandler<CreateFarmCommand, Response<string>>
    {
        private readonly IFarmService _farmService;
        private readonly IMapper _mapper;

        public FarmHandlerCommand(IFarmService farmService, IMapper mapper)
        {
            _mapper = mapper;
            _farmService = farmService;
        }

        public async Task<Response<string>> Handle(CreateFarmCommand request, CancellationToken cancellationToken)
        {
            var farm = _mapper.Map<Domain.Models.Farm>(request);

            // النتيجة الآن عبارة عن Enum وليست string
            var result = await _farmService.AddFarm(farm);

            switch (result)
            {
                case FarmServiceResult.OrganizationNotFound:
                    return NotFound<string>("Organization with this ID was not found."); // 404

                case FarmServiceResult.OrganizationIsNotFarm:
                    return UnprocessableEntity<string>("The selected organization is not of type 'Farm'."); // 422

                case FarmServiceResult.Success:
                    return Created("Farm created successfully"); // 201

                default:
                    return BadRequest<string>("Failed to create farm.");
            }
        }
    }
}