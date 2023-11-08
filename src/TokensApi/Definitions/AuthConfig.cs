using System.Security.Cryptography;

using TokensApi.Utils;

namespace TokensApi;

public class AuthSetting : ITryBuild<AuthConfig>
{
    public long? TokenLifetime { get; set; }
    public string? PrivateKey { get; set; }
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string?[]? Scopes { get; set; }

    public AuthConfig? TryBuild()
    {
        if (TokenLifetime is null || TokenLifetime < 60)
        {
            Console.Error.WriteLine($"{nameof(TokenLifetime)} must be atleast 60 seconds");
            return null;
        }
        var tokenLifetime = TokenLifetime.Value;

        if (string.IsNullOrWhiteSpace(PrivateKey))
        {
            Console.Error.WriteLine($"{nameof(PrivateKey)} must not be null/empty/whitespace");
            return null;
        }

        var rsa = RSA.Create();
        try
        {
            rsa.ImportFromPem(PrivateKey);
        }
        catch (ArgumentException ex)
        {
            Console.Error.WriteLine(ex.Message);
            Console.Error.WriteLine($"{PrivateKey} is invalid/tampered");
            return null;
        }

        if (string.IsNullOrWhiteSpace(ClientId))
        {
            Console.Error.WriteLine($"{nameof(ClientId)} cannot be null/empty/whitespace");
            return null;
        }

        if (string.IsNullOrWhiteSpace(ClientSecret))
        {
            Console.Error.WriteLine($"{nameof(ClientSecret)} cannot be null/empty/whitespace");
            return null;
        }

        if (Scopes is null || Scopes.Length == 0)
        {
            Console.Error.WriteLine($"{nameof(Scopes)} array must not be empty");
            return null;
        }

        var validScopes = new string[Scopes.Length];
        for (int i = 0; i < Scopes.Length; i++)
        {
            var scope = Scopes[i];
            if (scope is null || !Scope.ScopeExists(scope))
            {
                Console.Error.WriteLine($"invalid {nameof(Scope)} found at index {i}");
                return null;
            }
            validScopes[i] = scope;
        }

        return new AuthConfig(tokenLifetime, rsa, ClientId, ClientSecret, validScopes);
    }
}

public class AuthConfig
{
    public AuthConfig(long tokenLifetime, RSA privateKey, string clientId, string clientSecret, string[] scopes)
    {
        TokenLifetime = tokenLifetime;
        PrivateKey = privateKey;
        ClientId = clientId;
        ClientSecret = clientSecret;
        Scopes = scopes;
    }

    public long TokenLifetime { get; }
    public RSA PrivateKey { get; }
    public string ClientId { get; }
    public string ClientSecret { get; }
    public IEnumerable<string> Scopes { get; }
}