using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Base;
using Core.Features.Crop.Query.Models;
using Core.Features.Crop.Query.Result;
using MediatR;
using Service.Services.CropService;

namespace Core.Features.Crop.Query.Handler
{
    public class CropHandlerQuery : ResponseHandler, IRequestHandler<GetCropByIdQuery, Response<CropResponse>>,
        IRequestHandler<GetCropsListQuery, Response<List<CropResponse>>>
    {
        private readonly ICropService _cropService;
        private readonly IMapper _mapper;

        public CropHandlerQuery(ICropService cropService, IMapper mapper)
        {
            _cropService = cropService;
            _mapper = mapper;
        }
        // =====================================================
        // 5. Get By Id (Query)
        // =====================================================
        public async Task<Response<CropResponse>> Handle(GetCropByIdQuery request, CancellationToken cancellationToken)
        {
            var crop = await _cropService.GetCropByIdAsync(request.Id);
            if (crop == null) return NotFound<CropResponse>("Crop not found");

            var cropMapper = _mapper.Map<CropResponse>(crop);
            return Success(cropMapper);
        }

        // =====================================================
        // 6. Get All List (Query)
        // =====================================================
        public async Task<Response<List<CropResponse>>> Handle(GetCropsListQuery request, CancellationToken cancellationToken)
        {
            var crops = await _cropService.GetAllCropsAsync();
            var cropsMapper = _mapper.Map<List<CropResponse>>(crops);
            return Success(cropsMapper);
        }
    }
}
