using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Refit;

namespace Service.Services.ModelService
{
    public interface IPlantDiagnosisApi
    {
        [Multipart]
        [Post("/predict")]
        Task<DiagnosisResponse> PredictDiseaseAsync([AliasAs("file")] StreamPart file);
    }
}