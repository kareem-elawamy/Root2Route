using AutoMapper;
using Core.Base;
using Core.Features.authentication.Commands.Models;
using Domain.Enums;
using Domain.Models;
using Infrastructure.Repositories.OrganizationRepository;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Service;
using Service.Services;
using Service.Services.AuthenticationService;
using Service.Services.Email;

namespace Core.Features.authentication.Commands.Handler
{
    public class UserCommandHandler : ResponseHandler, IRequestHandler<AddUserCommand, Response<JwtAuthResult>>
    {
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOrganizationService _orgService;
        private readonly IOrganizationRepository _orgRepo;
        private readonly IAuthenticationService _authService; // إضافة سيرفيس التوكن

        public UserCommandHandler(IMapper mapper,
                                  UserManager<ApplicationUser> userManager,
                                  IOrganizationService orgService,
                                  IOrganizationRepository orgRepo,
                                  IAuthenticationService authService,
                                  IEmailService emailService)
        {
            _emailService = emailService;
            _userManager = userManager;
            _mapper = mapper;
            _orgService = orgService;
            _orgRepo = orgRepo;
            _authService = authService;
        }

        public async Task<Response<JwtAuthResult>> Handle(AddUserCommand request, CancellationToken cancellationToken)
        {
            // 1. تشيك تكرار الإيميل
            var user = await _userManager.FindByEmailAsync(request.Email!);
            if (user != null) return BadRequest<JwtAuthResult>("Email already exists");

            // 2. فتح Transaction
            // نعتمد على using لعمل Rollback تلقائي في حالة حدوث خطأ وعدم الوصول لـ Commit
            using var transaction = _orgRepo.BeginTransaction();

            try
            {
                // 3. إنشاء اليوزر
                var userIdentity = _mapper.Map<ApplicationUser>(request);
                userIdentity.UserName = request.Email; // تأكد من تعيين الـ UserName إذا كان يعتمد على الإيميل
                var createdResult = await _userManager.CreateAsync(userIdentity, request.Password!);

                if (!createdResult.Succeeded)
                {
                    // لا حاجة لعمل Rollback يدوياً، الـ using سيقوم بذلك عند الخروج
                    var error = createdResult.Errors.FirstOrDefault()?.Description;
                    return BadRequest<JwtAuthResult>(error ?? "Failed to create user");
                }
               
                // 5. توليد التوكن (تم نقلها قبل الـ Commit)
                // لضمان أنه في حالة فشل التوكن، لا يتم حفظ اليوزر في الداتا بيز
                var accessToken = await _authService.GenerateToken(userIdentity);

                // 6. تثبيت التغييرات (آخر خطوة تماماً)
                transaction.Commit();

                var otpCode = await _userManager.GenerateEmailConfirmationTokenAsync(userIdentity);
                var emailBody = GetOtpHtmlTemplate(userIdentity.FullName, otpCode);
                await _emailService.SendEmailAsync(userIdentity.Email!, "تفعيل حساب Root2Route", emailBody);
                // 7. إرجاع النتيجة
                return Success(accessToken);
            }
            catch (Exception ex)
            {
                // لا تقم بعمل Rollback يدوياً إذا كنت تستخدم using، إلا إذا كنت تريد التعامل مع الخطأ وإكمال التنفيذ
                // transaction.Rollback(); // يفضل إزالتها لتجنب Exception إذا كان الخطأ في الـ Commit نفسه

                return BadRequest<JwtAuthResult>("An error occurred: " + ex.Message);
            }

        }
        private string GetOtpHtmlTemplate(string userName, string otpCode)
        {
            return $@"
    <!DOCTYPE html>
    <html lang='ar' dir='rtl'>
    <head>
        <meta charset='UTF-8'>
        <style>
            body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f4f7f6; margin: 0; padding: 0; }}
            .container {{ max-width: 600px; margin: 20px auto; background-color: #ffffff; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 15px rgba(0,0,0,0.1); border: 1px solid #e0e0e0; }}
            .header {{ background-color: #2d6a4f; padding: 30px; text-align: center; color: white; }}
            .header h1 {{ margin: 0; font-size: 28px; letter-spacing: 1px; }}
            .content {{ padding: 40px; text-align: center; color: #333; }}
            .content h2 {{ color: #1b4332; margin-bottom: 20px; }}
            .otp-box {{ background-color: #f1f8e9; border: 2px dashed #40916c; border-radius: 8px; padding: 20px; margin: 30px 0; }}
            .otp-code {{ font-size: 36px; font-weight: bold; color: #2d6a4f; letter-spacing: 10px; }}
            .footer {{ background-color: #f8f9fa; padding: 20px; text-align: center; font-size: 13px; color: #666; border-top: 1px solid #eeeeee; }}
            .footer p {{ margin: 5px 0; }}
        </style>
    </head>
    <body>
        <div class='container'>
            <div class='header'>
                <h1>Root2Route</h1>
            </div>
            <div class='content'>
                <h2>مرحباً {userName}</h2>
                <p>سعداء بانضمامك إلى منصتنا. يرجى استخدام كود التحقق التالي لإتمام عملية تسجيل حسابك:</p>
                <div class='otp-box'>
                    <div class='otp-code'>{otpCode}</div>
                </div>
                <p>هذا الكود صالح لمدة 15 دقيقة فقط.</p>
                <p>إذا لم تقم بإنشاء حساب، يمكنك تجاهل هذا البريد الإلكتروني.</p>
            </div>
            <div class='footer'>
                <p>© {DateTime.Now.Year} Root2Route - دعم الزراعة المستدامة</p>
                <p>هذه رسالة تلقائية، يرجى عدم الرد عليها.</p>
            </div>
        </div>
    </body>
    </html>";
        }
    }
}