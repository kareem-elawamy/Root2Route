using System;
using System.Collections.Generic;
using System.Text;

namespace Service;

public class JwtAuthResult
{
    public string AccessToken { get; set; } = null!;
    public string? RefreshToken { get; set; } 
    public DateTime ExpireAt { get; set; }

    public string? FullName { get; set; }
    public string? UserType { get; set; }
}
