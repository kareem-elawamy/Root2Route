using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Service.Services.Email;

namespace Core.Features.Authentication.Commands.Handler
{
    public class ForgetPasswordHandler : ResponseHandler,
        IRequestHandler<SendResetPasswordOtpCommand, Response<string>>,
        IRequestHandler<ResetPasswordWithOtpCommand, Response<string>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;

        public ForgetPasswordHandler(
            UserManager<ApplicationUser> userManager,
            IEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
        }

        public async Task<Response<string>> Handle(
       SendResetPasswordOtpCommand request,
       CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
                return NotFound<string>("User not found");

            if (!user.EmailConfirmed)
                return BadRequest<string>("Email not confirmed");

            var otpCode = await _userManager.GenerateTwoFactorTokenAsync(
                        user,
                        TokenOptions.DefaultEmailProvider
                    );

            var emailBody = GetOtpHtmlTemplate(user.FullName, otpCode);

            await _emailService.SendEmailAsync(
                user.Email!,
                "Reset Password Code - Root2Route",
                emailBody
            );

            return Success("Password reset code sent to email");
        }

        public async Task<Response<string>> Handle(
    ResetPasswordWithOtpCommand request,
    CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
                return NotFound<string>("User not found");

            var result = await _userManager.ResetPasswordAsync(
                user,
                request.Otp,
                request.NewPassword
            );

            if (!result.Succeeded)
                return BadRequest<string>("Invalid OTP or password reset failed");

            return Success("Password changed successfully");
        }

        private string GetOtpHtmlTemplate(string userName, string otpCode)
        {
            return $@"

<!DOCTYPE html>

<html lang='ar' dir='rtl'>
<head>
    <meta charset='UTF-8'>
    <style>
        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background-color: #f4f7f6;
            margin: 0;
            padding: 0;
        }}

```
    .container {{
        max-width: 600px;
        margin: 20px auto;
        background-color: #ffffff;
        border-radius: 12px;
        overflow: hidden;
        box-shadow: 0 4px 15px rgba(0,0,0,0.1);
        border: 1px solid #e0e0e0;
    }}

    .header {{
        background-color: #2d6a4f;
        padding: 30px;
        text-align: center;
        color: white;
    }}

    .header h1 {{
        margin: 0;
        font-size: 28px;
        letter-spacing: 1px;
    }}

    .content {{
        padding: 40px;
        text-align: center;
        color: #333;
    }}

    .content h2 {{
        color: #1b4332;
        margin-bottom: 20px;
    }}

    .otp-box {{
        background-color: #f1f8e9;
        border: 2px dashed #40916c;
        border-radius: 8px;
        padding: 20px;
        margin: 30px 0;
    }}

    .otp-code {{
        font-size: 34px;
        font-weight: bold;
        color: #2d6a4f;
        letter-spacing: 8px;
    }}

    .footer {{
        background-color: #f8f9fa;
        padding: 20px;
        text-align: center;
        font-size: 13px;
        color: #666;
        border-top: 1px solid #eeeeee;
    }}

    .footer p {{
        margin: 5px 0;
    }}
</style>
```

</head>

<body>

<div class='container'>

```
<div class='header'>
    <h1>Root2Route</h1>
</div>

<div class='content'>
    <h2>مرحباً {userName}</h2>

    <p>
        تلقينا طلبًا لإعادة تعيين كلمة المرور الخاصة بحسابك.
        استخدم الكود التالي لإكمال عملية تغيير كلمة المرور:
    </p>

    <div class='otp-box'>
        <div class='otp-code'>{otpCode}</div>
    </div>

    <p>
        هذا الكود صالح لمدة <strong>15 دقيقة فقط</strong>.
    </p>

    <p>
        إذا لم تقم بطلب تغيير كلمة المرور، يمكنك تجاهل هذا البريد الإلكتروني.
    </p>

</div>

<div class='footer'>
    <p>© {DateTime.Now.Year} Root2Route</p>
    <p>هذه رسالة تلقائية، يرجى عدم الرد عليها.</p>
</div>
```

</div>

</body>
</html>";
        }

    }
}