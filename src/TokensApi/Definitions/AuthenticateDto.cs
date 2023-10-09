using TokensApi.Utils;

namespace TokensApi;

public class AuthenticationRequestDto : ITryBuild<AuthenticationRequest>
{
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }

    /// <summary>
    /// Tries to build a valid <see cref="AuthenticationRequest"/>
    /// </summary>
    /// <returns>A valid <see cref="AuthenticationRequest"/> or null</returns>
    public AuthenticationRequest? TryBuild()
    {
        return string.IsNullOrWhiteSpace(ClientId) || string.IsNullOrWhiteSpace(ClientSecret)
            ? null
            : new AuthenticationRequest(ClientId, ClientSecret);
    }
}