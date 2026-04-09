using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;

namespace Core.Features.PlantInfo.Commands.Models
{
    public class UploadPlantInfoImageCommand : IRequest<Response<string>>
    {
        public Guid PlantInfoId { get; set; }
        public IFormFile Image { get; set; }
    }
}