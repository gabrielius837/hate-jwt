using System.Security.Cryptography;

namespace TokensApi;

public class AuthenticationSettings : ITryBuild<AuthenticationConfig>
{
    public long? TokenLifetime { get; set; }
    public string? PublicKey { get; set; }
    public ProfileSetting[]? ProfileSettings { get; set; }
    public AuthenticationConfig? TryBuild()
    {
        if (TokenLifetime is null || TokenLifetime < 60)
        {
            Console.Error.WriteLine($"{nameof(TokenLifetime)} must be atleast 60 seconds");
            return null;
        }
        var tokenLifetime = TokenLifetime.Value;

        if (string.IsNullOrWhiteSpace(PublicKey))
        {
            Console.Error.WriteLine($"{nameof(PublicKey)} must not be null/empty/whitespace");
            return null;
        }

        var rsa = RSA.Create();
        try
        {
            rsa.ImportFromPem(PublicKey);
        }
        catch (ArgumentException ex)
        {
            Console.Error.WriteLine("invalid private key detected");
            Console.Error.WriteLine(ex.ToString());
            return null;
        }

        if (ProfileSettings is null || ProfileSettings.Length == 0)
        {
            Console.Error.WriteLine($"{nameof(ProfileSettings)} array must not be empty");
            return null;
        }

        var profileSet = new HashSet<ProfileConfig>
        (
            ProfileSettings.Length,
            new ProfileConfigComparer()
        );

        for (int i = 0; i < ProfileSettings.Length; i++)
        {
            var profile = ProfileSettings[i];
            var profileConfig = profile.TryBuild();
            if (profileConfig is null)
            {
                Console.Error.WriteLine($"invalid {nameof(ProfileSetting)} entry found at index {i}");
                return null;
            }

            // handle a duplicating client id and secret
            var added = profileSet.Add(profileConfig);
            if (!added)
            {
                Console.Error.WriteLine($"{nameof(ProfileSettings)} constains a duplicate of ClientId: {profile.ClientId} ClientSecret: {profile.ClientSecret}");
                return null;
            }
        }

        return new AuthenticationConfig(tokenLifetime, rsa, profileSet.ToArray());
    }
}

public class AuthenticationConfig
{
    public AuthenticationConfig(long tokenLifetime, RSA publicKey, ProfileConfig[] profiles)
    {
        TokenLifetime = tokenLifetime;
        PublicKey = publicKey;
        ProfileConfigs = profiles;
    }

    public long TokenLifetime { get; }
    public RSA PublicKey { get; }
    public ProfileConfig[] ProfileConfigs { get; }
}