using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Features.ModelAI.Query.Models;
using Core.Features.ModelAI.Query.Result;
using Service.Services.ModelService;

namespace Core.Features.ModelAI.Query.Handler
{
    public class ModelAnalysisHandler : ResponseHandler, IRequestHandler<AnalyzePlantImageQuery, Response<AnalyzePlantImageResponse>>
    {
        private readonly IModelService _modelService;
        private readonly IMapper _mapper;

        public ModelAnalysisHandler(IModelService modelService, IMapper mapper)
        {
            _modelService = modelService;
            _mapper = mapper;
        }
        public async Task<Response<AnalyzePlantImageResponse>> Handle(AnalyzePlantImageQuery request, CancellationToken cancellationToken)
        {
            // 1. تنفيذ العملية من خلال الـ Service
            // الـ Service هنا هو اللي بيكلم الـ Python API وجيميني
            var result = await _modelService.GetExpertAdvice(request.ImageFile);

            if (result == null)
                return BadRequest<AnalyzePlantImageResponse>("حدث خطأ أثناء معالجة الصورة");

            if (!result.LeafDetected)
                return BadRequest<AnalyzePlantImageResponse>("لم يتم اكتشاف ورقة نبات، يرجى تصوير الورقة بشكل أوضح");

            // 2. عمل الـ Mapping للنتيجة
            var mappedResponse = _mapper.Map<AnalyzePlantImageResponse>(result);

            return Success(mappedResponse);
        }
    }
}