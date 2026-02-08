using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Base;
using Domain.Enums;
using MediatR;

namespace Core.Features.Crop.Command.Models
{
    public class CreateCropCommand : IRequest<Response<string>>
    {
        public Guid FarmId { get; set; }
        public double PlantedArea { get; set; }
        public DateTime PlantingDate { get; set; }
        public DateTime? ExpectedHarvestDate { get; set; }
        public YieldUnit YieldUnit { get; set; }
        public Guid? PlantInfoId { get; set; }

    }
}