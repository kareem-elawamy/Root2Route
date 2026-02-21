using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Features.ModelAI.Query.Result;
using Microsoft.AspNetCore.Http;

namespace Core.Features.ModelAI.Query.Models
{
    public class AnalyzePlantImageQuery : IRequest<Response<AnalyzePlantImageResponse>>
    {
        public IFormFile ImageFile { get; set; }
    }
}