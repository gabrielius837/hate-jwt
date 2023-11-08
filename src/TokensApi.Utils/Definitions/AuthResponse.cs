namespace TokensApi.Utils;

public class AuthResponse
{

    public long ExpiresAt { get; set; }
    public string Token { get; set; } = string.Empty;
    public string Scheme { get; set; } = string.Empty;
}