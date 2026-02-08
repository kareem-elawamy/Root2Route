using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Base;
using Core.Features.Crop.Command.Models;
using Domain.Enums;
using MediatR;
using Service.Services.CropService;

namespace Core.Features.Crop.Command.Handler
{
    public class CropHandlerCommand : ResponseHandler,
        IRequestHandler<CreateCropCommand, Response<string>>,
        IRequestHandler<UpdateCropCommand, Response<string>>,
        IRequestHandler<DeleteCropCommand, Response<string>>,
        IRequestHandler<RegisterHarvestCommand, Response<string>>
    {
        private readonly ICropService _cropService;
        private readonly IMapper _mapper;

        public CropHandlerCommand(ICropService cropService, IMapper mapper)
        {
            _cropService = cropService;
            _mapper = mapper;
        }
        public async Task<Response<string>> Handle(CreateCropCommand request, CancellationToken cancellationToken)
        {
            var crop = _mapper.Map<Domain.Models.Crop>(request);
            var result = await _cropService.AddCropAsync(crop);

            return result switch
            {
                CropServiceResult.Success => Created("Crop created successfully"),
                CropServiceResult.FarmNotFound => NotFound<string>("The specified Farm was not found"),
                CropServiceResult.InvalidDates => BadRequest<string>("Expected Harvest Date cannot be before Planting Date"),
                _ => BadRequest<string>("Failed to create crop")
            };
        }

        public async Task<Response<string>> Handle(DeleteCropCommand request, CancellationToken cancellationToken)
        {
            var result = await _cropService.DeleteCropAsync(request.Id);

            return result switch
            {
                CropServiceResult.Success => Success("Crop deleted successfully"), // تم التحويل لـ Soft Delete في السرفيس
                CropServiceResult.NotFound => NotFound<string>("Crop not found"),
                _ => BadRequest<string>("Failed to delete crop")
            };
        }

        public async Task<Response<string>> Handle(UpdateCropCommand request, CancellationToken cancellationToken)
        {
            var crop = _mapper.Map<Domain.Models.Crop>(request);
            var result = await _cropService.UpdateCropAsync(crop);

            return result switch
            {
                CropServiceResult.Success => Success("Crop updated successfully"),
                CropServiceResult.NotFound => NotFound<string>("Crop not found"),
                CropServiceResult.InvalidDates => BadRequest<string>("Expected Harvest Date cannot be before Planting Date"),
                _ => BadRequest<string>("Failed to update crop")
            };
        }

        public async Task<Response<string>> Handle(RegisterHarvestCommand request, CancellationToken cancellationToken)
        {
            var result = await _cropService.RegisterHarvestAsync(request.CropId, request.ActualYieldQuantity);

            return result switch
            {
                CropServiceResult.Success => Success("Harvest registered successfully"),
                CropServiceResult.NotFound => NotFound<string>("Crop not found"),
                _ => BadRequest<string>("Failed to register harvest")
            };
        }
    }
}