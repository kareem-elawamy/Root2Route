using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Service.Services.ModelService
{
    public interface IModelService
    {
        Task<DiagnosisResponse> GetExpertAdvice(IFormFile image);
    }
}