namespace TokensApi.Utils;

public class AuthenticationRequest
{
    public AuthenticationRequest(string clientId, string clientSecret)
    {
        ClientId = clientId;
        ClientSecret = clientSecret;
    }

    public string ClientId { get; }
    public string ClientSecret { get; }
}