using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Refit;
using Service.Services.AIService;

namespace Service.Services.ModelService
{
    public class ModelService : IModelService
    {
        private readonly IPlantDiagnosisApi _pythonApiClient;
        private readonly IAIService _aiService;
        public ModelService(IPlantDiagnosisApi pythonApiClient, IAIService aiService)
        {
            _aiService = aiService;
            _pythonApiClient = pythonApiClient;
        }
        public async Task<DiagnosisResponse> GetExpertAdvice(IFormFile image)
        {
            if (image == null || image.Length == 0)
                throw new ArgumentException("No image provided.");

            if (!image.ContentType.StartsWith("image/"))
                throw new ArgumentException("File must be an image.");

            try
            {
                using var stream = image.OpenReadStream();
                var streamPart = new StreamPart(stream, image.FileName, image.ContentType);

                // 1. استدعاء Python API
                var diseaseResult = await _pythonApiClient.PredictDiseaseAsync(streamPart);

                // 2. إذا لم يتم اكتشاف ورقة، ارجع النتيجة فوراً
                if (!diseaseResult.LeafDetected)
                {
                    return diseaseResult;
                }

                // 3. استدعاء Gemini للحصول على النصيحة
                var advice = await _aiService.GetAdvice(diseaseResult.Prediction);

                // 4. دمج النصيحة في الكائن المرجوع (هام جداً)
                diseaseResult.ExpertAdvice = advice;

                return diseaseResult;
            }
            catch (Exception ex)
            {
                // هنا ممكن نعمل لوج للخطأ
                Console.WriteLine($"Error in ModelService: {ex.Message}");
                throw new Exception("حدث خطأ أثناء معالجة الصورة. يرجى المحاولة مرة أخرى.");
            }
        }
    }
}