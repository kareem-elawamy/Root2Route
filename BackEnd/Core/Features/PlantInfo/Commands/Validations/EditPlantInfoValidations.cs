using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Features.PlantInfo.Commands.Models;
using FluentValidation;
using Service.Services.PlantInfoService;

namespace Core.Features.PlantInfo.Commands.Validations
{
    public class EditPlantInfoValidations : AbstractValidator<EditPlantInfoCommand>
    {
        private readonly IPlantInfoService _plantInfoService;
        public EditPlantInfoValidations(IPlantInfoService plantInfoService)
        {
            _plantInfoService = plantInfoService;
            ApplyValidationRules();

        }
        public void ApplyValidationRules()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Plant Info Id is required.");
            RuleFor(x => x.Name).NotEmpty().WithMessage("Plant Name is required.");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required.");
            RuleFor(x => x.IdealSoil).NotEmpty().WithMessage("Ideal Soil information is required.");
            RuleFor(x => x.PlantingSeason).NotEmpty().WithMessage("Planting Season information is required.");
            RuleFor(x => x.ScientificName).NotEmpty().WithMessage("Scientific Name is required.");
            RuleFor(x => x.MedicalBenefits).NotEmpty().WithMessage("Medical Benefits information is required.");
        }

    }
}