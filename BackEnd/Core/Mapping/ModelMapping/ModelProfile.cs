using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Features.ModelAI.Query.Result;
using Service;

namespace Core.Mapping.ModelMapping
{
    public class ModelProfile : Profile
    {
        public ModelProfile()
        {
            CreateMap<DiagnosisResponse, AnalyzePlantImageResponse>();
        }
    }
}