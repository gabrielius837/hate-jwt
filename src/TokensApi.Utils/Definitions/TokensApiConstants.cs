namespace TokensApi.Utils;

public static class TokensApiConstants
{
    /// <summary>
    /// Scheme that should be used
    /// in combination with jwt token
    /// </summary>
    public const string JwtAuthScheme = "Bearer";
    /// <summary>
    /// Scheme that should be used
    /// in combination with jwt token
    /// </summary>
    public const string Issuer = "tokens-api";
    /// <summary>
    /// Key for scope claim
    /// </summary>
    public const string Scope = "scope";
}