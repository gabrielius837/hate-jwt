using System.Diagnostics.CodeAnalysis;

namespace TokensApi;

public class ProfileConfigComparer : IEqualityComparer<ProfileConfig>
{
    public bool Equals(ProfileConfig? x, ProfileConfig? y)
    {
        // if references are equal - true
        return ReferenceEquals(x, y) ||
        // if any of profiles are null - false
            x is not null && y is not null &&
        // if both pairs of client id and client secret equals - true
            x.ClientId == y.ClientId && x.ClientSecret == y.ClientSecret;
    }

    public int GetHashCode([DisallowNull] ProfileConfig obj)
    {
        var clientIdHashCode = obj.ClientId.GetHashCode();
        var clientSecretHashCode = obj.ClientSecret.GetHashCode();

        int hash = 17;
        hash = hash * 43 + clientIdHashCode;
        hash = hash * 43 + clientSecretHashCode;
        return hash;
    }
}