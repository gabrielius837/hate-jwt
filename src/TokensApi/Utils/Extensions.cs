using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;

using TokensApi.Utils;

namespace TokensApi;

public static class TokensApiExtensions
{
    public const string AuthSettings = nameof(AuthSettings);

    public static AuthConfig[]? TryBuildAuthConfigs(AuthSetting[] settings)
    {
        if (settings.Length == 0)
        {
            Console.Error.WriteLine($"{nameof(AuthSetting)} array is empty");
            return null;
        }

        var authConfigSet = new HashSet<AuthConfig>(settings.Length, new AuthConfigComparer());

        for (int i = 0; i < settings.Length; i++)
        {
            var authConfig = TryBuildAuthConfig(settings[i]);
            if (authConfig is null || !authConfigSet.Add(authConfig))
            {
                Console.Error.WriteLine($"invalid {nameof(AuthSetting)} at index {i}");
                return null;
            }
        }

        return authConfigSet.ToArray();
    }

    private static AuthConfig? TryBuildAuthConfig(AuthSetting? setting)
    {
        if (setting is null)
        {
            Console.Error.WriteLine($"{nameof(AuthSetting)} cannot be null");
            return null;
        }

        if (setting.TokenLifetime is null || setting.TokenLifetime < 60)
        {
            Console.Error.WriteLine($"{nameof(setting.TokenLifetime)} must be atleast 60 seconds");
            return null;
        }
        var tokenLifetime = setting.TokenLifetime.Value;

        if (string.IsNullOrWhiteSpace(setting.PrivateKey))
        {
            Console.Error.WriteLine($"{nameof(setting.PrivateKey)} must not be null/empty/whitespace");
            return null;
        }

        var rsa = RSA.Create();
        try
        {
            rsa.ImportFromPem(setting.PrivateKey);
        }
        catch (ArgumentException ex)
        {
            Console.Error.WriteLine(ex.Message);
            Console.Error.WriteLine($"{setting.PrivateKey} is invalid/tampered");
            return null;
        }

        if (string.IsNullOrWhiteSpace(setting.ClientId))
        {
            Console.Error.WriteLine($"{nameof(setting.ClientId)} cannot be null/empty/whitespace");
            return null;
        }

        if (string.IsNullOrWhiteSpace(setting.ClientSecret))
        {
            Console.Error.WriteLine($"{nameof(setting.ClientSecret)} cannot be null/empty/whitespace");
            return null;
        }

        if (setting.Scopes is null || setting.Scopes.Length == 0)
        {
            Console.Error.WriteLine($"{nameof(setting.Scopes)} array must not be empty");
            return null;
        }

        var scopeSet = new HashSet<string>(setting.Scopes.Length);

        for (int i = 0; i < setting.Scopes.Length; i++)
        {
            var scope = setting.Scopes[i];
            if (scope is null || !Scope.ScopeExists(scope) || !scopeSet.Add(scope))
            {
                Console.Error.WriteLine($"invalid {nameof(Scope)} found at index {i}");
                return null;
            }
        }

        return new AuthConfig
        (
            tokenLifetime,
            rsa,
            setting.ClientId,
            setting.ClientSecret,
            scopeSet.ToArray()
        );
    }

    private class AuthConfigComparer : IEqualityComparer<AuthConfig>
    {
        public bool Equals(AuthConfig? x, AuthConfig? y)
        {
            // if references are equal - true
            return ReferenceEquals(x, y) ||
                // if any of profiles are null - false
                x is not null && y is not null &&
                // if both pairs of client id and client secret equals - true
                x.ClientId == y.ClientId && x.ClientSecret == y.ClientSecret;
        }

        public int GetHashCode([DisallowNull] AuthConfig obj)
        {
            var clientIdHashCode = obj.ClientId.GetHashCode();
            var clientSecretHashCode = obj.ClientSecret.GetHashCode();

            int hash = 17;
            hash = hash * 43 + clientIdHashCode;
            hash = hash * 43 + clientSecretHashCode;
            return hash;
        }
    }
}