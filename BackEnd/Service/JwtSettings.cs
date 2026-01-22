using System;
using System.Collections.Generic;
using System.Text;

namespace Service;

public class JwtSettings
{
    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty; // تم التعديل من Issure
    public string Audience { get; set; } = string.Empty;
    public double AccessTokenExpireTime { get; set; }
    public double RefreshTokenExpireTime { get; set; }
}
