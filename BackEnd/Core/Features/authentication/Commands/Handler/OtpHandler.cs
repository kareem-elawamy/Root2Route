using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Service;
using Service.Services.Email;

namespace Core.Features.Authentication.Commands.Handler
{
    public class OtpHandler : ResponseHandler, IRequestHandler<VerifyOtpCommand, Response<JwtAuthResult>>,
        IRequestHandler<ResendOtpCommand, Response<string>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        public OtpHandler(UserManager<ApplicationUser> userManager, IEmailService emailService)
        {
            _emailService = emailService;
            _userManager = userManager;
        }
        public Task<Response<JwtAuthResult>> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
        {
            var user = _userManager.FindByEmailAsync(request.Email).Result;
            if (user == null)
                return Task.FromResult(BadRequest<JwtAuthResult>("User not found"));
            var isValidOtp = _userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultEmailProvider, "EmailConfirmation", request.Otp).Result;
            if (!isValidOtp)
                return Task.FromResult(BadRequest<JwtAuthResult>("Invalid OTP"));
            var result = _userManager.ConfirmEmailAsync(user, request.Otp).Result;
            if (!result.Succeeded)
                return Task.FromResult(BadRequest<JwtAuthResult>("Failed to confirm email"));
            return Task.FromResult(Success(new JwtAuthResult
            {
                FullName
                = user.FullName,
            }));
        }

        public async Task<Response<string>> Handle(ResendOtpCommand request, CancellationToken cancellationToken)
        {
            // 1. البحث عن المستخدم
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null) return NotFound<string>("User not found");

            // 2. التأكد أن الإيميل لم يتم تفعيله بالفعل
            if (user.EmailConfirmed) return BadRequest<string>("Email is already confirmed");

            // 3. توليد كود جديد (نفس المنطق المستخدم في التسجيل)
            var otpCode = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            // 4. إرسال الإيميل
            var emailBody = GetOtpHtmlTemplate(user.FullName, otpCode);
            await _emailService.SendEmailAsync(user.Email!, "إعادة إرسال كود التحقق - Root2Route", emailBody);

            return Success("تم إعادة إرسال كود التحقق إلى بريدك الإلكتروني.");
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