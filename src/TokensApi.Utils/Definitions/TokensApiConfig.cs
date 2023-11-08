namespace TokensApi.Utils;

public class TokensApiSetting : ITryBuild<TokensApiConfig>
{
    public string? TokensApiUrl { get; set; }
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }

    public TokensApiConfig? TryBuild()
    {
        if (
            !Uri.TryCreate(TokensApiUrl, UriKind.Absolute, out Uri? tokensApiUrl) ||
            tokensApiUrl == null ||
            (tokensApiUrl.Scheme != Uri.UriSchemeHttp && tokensApiUrl.Scheme != Uri.UriSchemeHttps)
        )
        {
            Console.Error.WriteLine($"{nameof(TokensApiUrl)} is not valid url");
            return null;
        }

        if (ClientId is null || !Client.ClientExists(ClientId))
        {
            Console.Error.WriteLine($"{nameof(ClientId)} is not valid");
            return null;
        }

        if (string.IsNullOrWhiteSpace(ClientSecret))
        {
            Console.Error.WriteLine($"{nameof(ClientSecret)} is not valid");
            return null;
        }

        return new TokensApiConfig(tokensApiUrl, ClientId, ClientSecret);
    }
}

public class TokensApiConfig
{
    public TokensApiConfig(Uri tokensApiUrl, string clientId, string clientSecret)
    {
        TokensApiUrl = tokensApiUrl;
        ClientId = clientId;
        ClientSecret = clientSecret;
    }

    public Uri TokensApiUrl { get; }
    public string ClientId { get; }
    public string ClientSecret { get; }
}