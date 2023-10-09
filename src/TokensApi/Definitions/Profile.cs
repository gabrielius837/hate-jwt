using System.Collections.Immutable;

namespace TokensApi;

public class ProfileSetting : ITryBuild<ProfileConfig>
{
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string[]? Scopes { get; set; }
    public ProfileConfig? TryBuild()
    {
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

        for (int i = 0; i < Scopes.Length; i++)
        {
            var scope = Scopes[i];
            if (string.IsNullOrWhiteSpace(scope) || !Utils.Scopes.ScopeExists(scope))
            {
                Console.Error.WriteLine($"invalid Scope found at index {i}");
                return null;
            }
        }

        return new ProfileConfig(ClientId, ClientSecret, Scopes);
    }
}

public class ProfileConfig
{
    public ProfileConfig(string clientId, string clientSecret, IEnumerable<string> scopes)
    {
        ClientId = clientId;
        ClientSecret = clientSecret;
        Scopes = scopes;
    }

    public string ClientId { get; }
    public string ClientSecret { get; }
    public IEnumerable<string> Scopes { get; }
}
