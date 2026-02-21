using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Mscc.GenerativeAI;
using Mscc.GenerativeAI.Types;

namespace Service.Services.AIService
{
    public class AIService : IAIService
    {
        private readonly string _apiKey;

        public AIService(IConfiguration configuration)
        {
            _apiKey = configuration["ExternalApis:GeminiApiKey"];
        }

        public async Task<string> GetAdvice(string diseaseName)
        {
            try
            {
                var googleAI = new GoogleAI(_apiKey);

                var model = googleAI.GenerativeModel(
                    Model.Gemini25Flash,
                    new GenerationConfig
                    {
                        // يقلل حجم الرد
                        Temperature = 0.4f,      // يقلل الإبداع = سرعة أعلى
                        TopP = 0.8f,
                        TopK = 20
                    });

                string prompt = $@"
أنت خبير زراعي محترف.
تم تشخيص النبات بمرض: {diseaseName}.
اكتب نصيحة مختصرة ومنظمة في نقاط:
- وصف المرض
- الأسباب
- العلاج
- الوقاية
اجعل الرد لا يتجاوز 400 كلمة.";

                var response = await model.GenerateContent(prompt);

                // اطبع الرد كامل للتشخيص

                if (response?.Candidates == null || !response.Candidates.Any())
                {
                    throw new Exception("Gemini returned no candidates.");
                }

                var result = response.Candidates
                    .First()
                    .Content?
                    .Parts?
                    .FirstOrDefault()?
                    .Text;

                if (string.IsNullOrWhiteSpace(result))
                {
                    throw new Exception("Gemini returned empty text.");
                }

                return result;
            }
            catch (Exception)
            {
                throw new Exception("خطأ أثناء التواصل مع Gemini API.");
            }
        }
    }
}