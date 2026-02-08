using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Base;
using MediatR;

namespace Core.Features.Crop.Command.Models
{
    public class RegisterHarvestCommand : IRequest<Response<string>>
    {
        public Guid CropId { get; set; }
        public double ActualYieldQuantity { get; set; }
    }
}