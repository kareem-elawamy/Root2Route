using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using Service.Services;
using Service.Services.AIService;
using Service.Services.AuthenticationService;
using Service.Services.AuthorizationService;
using Service.Services.Email;
using Service.Services.FileService;
using Service.Services.ModelService;
using Service.Services.OrganizationMemberService;
using Service.Services.OrganizationRoleService;
using Service.Services.PlantGuideStepService;
using Service.Services.PlantInfoService;
using Service.Services.ProductService;

namespace Service;

public static class ModuleServiceDependencies
{
    public static IServiceCollection AddServiceDependencies(this IServiceCollection services)
    {
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<IPlantGuideStepService, PlantGuideStepService>();
        services.AddScoped<IPlantInfoService, PlantInfoService>();
        services.AddScoped<IOrganizationService, OrganizationService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IAuthorizationService, AuthorizationService>();
        services.AddScoped<IOrganizationRoleService, OrganizationRoleService>();
        services.AddScoped<IOrganizationMemberService, OrganizationMemberService>();

        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IEmailService, EmailService>();
        return services;
    }
    public static IServiceCollection AddModelServiceDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. جلب رابط الـ Python API من الإعدادات
        var pythonApiUrl = configuration["ExternalApis:PlantPythonApi"];

        // 2. تسجيل Refit Client للاتصال بموديل Python (Hunter & Doctor)
        services.AddRefitClient<IPlantDiagnosisApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(pythonApiUrl!));

        // 3. تسجيل ModelService التي ستدير العمليات
        services.AddScoped<IModelService, ModelService>();

        // 4. تسجيل خدمة Gemini كـ Dependency ليستخدمها ModelService
        // ملاحظة: تأكد من إضافة الـ AIExpertService أو دمج منطقها داخل ModelService
        services.AddScoped<IAIService, AIService>();

        return services;
    }
}
