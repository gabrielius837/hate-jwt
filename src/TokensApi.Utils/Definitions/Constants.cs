using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace TokensApi.Utils;

public static class Constants
{
    /// <summary>
    /// Scheme that should be used
    /// in combination with jwt token
    /// </summary>
    public const string JwtAuthScheme = JwtBearerDefaults.AuthenticationScheme;
    /// <summary>
    /// Scheme that should be used
    /// in combination with jwt token
    /// </summary>
    public const string Issuer = "tokens-api";
}