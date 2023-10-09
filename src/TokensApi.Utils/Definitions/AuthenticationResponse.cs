namespace TokensApi.Utils;

public class AuthenticationResponse
{
    public AuthenticationResponse(long expiresAt, string token, string schema)
    {
        ExpiresAt = expiresAt;
        Token = token;
        Schema = schema;
    }

    public long ExpiresAt { get; }
    public string Token { get; }
    public string Schema { get; }
}