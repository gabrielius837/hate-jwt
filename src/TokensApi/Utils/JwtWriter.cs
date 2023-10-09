using System.IdentityModel.Tokens.Jwt;

using Microsoft.IdentityModel.Tokens;

using TokensApi.Utils;

namespace TokensApi;

public interface IJwtWriter
{
    AuthenticationResponse? IssueToken(AuthenticationRequest req);
}

public class JwtWriter : IJwtWriter
{
    private readonly ILogger<JwtWriter> _logger;
    private readonly AuthenticationConfig _config;
    private readonly JwtSecurityTokenHandler _handler;

    public JwtWriter(ILogger<JwtWriter> logger, AuthenticationConfig config)
    {
        _logger = logger;
        _config = config;
        _handler = new JwtSecurityTokenHandler();
    }

    public AuthenticationResponse? IssueToken(AuthenticationRequest req)
    {
        var profile = _config.ProfileConfigs.FirstOrDefault(x =>
            x.ClientId == req.ClientId &&
            x.ClientSecret == req.ClientSecret
        );

        if (profile is null)
        {
            return null;
        }

        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var exp = now + _config.TokenLifetime;

        var header = new JwtHeader
        (
            new SigningCredentials
            (
                new RsaSecurityKey(_config.PrivateKey),
                SecurityAlgorithms.RsaSha256
            )
        );

        var payload = new JwtPayload
        {
            [JwtRegisteredClaimNames.Iss] = Constants.Issuer,
            [JwtRegisteredClaimNames.Sub] = profile.ClientId,
            [JwtRegisteredClaimNames.Iat] = now,
            [JwtRegisteredClaimNames.Exp] = exp,
            ["scopes"] = profile.Scopes
        };

        var token = new JwtSecurityToken
        (
            header,
            payload
        );

        var serializedToken = _handler.WriteToken(token);
        if (serializedToken is null)
        {
            _logger.LogError("failed to write token {clientId} {clientSecret}", profile.ClientId, profile.ClientSecret);
            return null;
        }

        return new AuthenticationResponse(exp, serializedToken, Constants.JwtAuthScheme);
    }
}