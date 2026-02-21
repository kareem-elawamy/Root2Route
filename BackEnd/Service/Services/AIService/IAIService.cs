using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Services.AIService
{
    public interface IAIService
    {
        Task<string> GetAdvice(string diseaseName);
    }
}