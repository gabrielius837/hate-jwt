using System.IdentityModel.Tokens.Jwt;

using Microsoft.IdentityModel.Tokens;

using TokensApi.Utils;

namespace TokensApi;

public interface IJwtWriter
{
    AuthResponse? IssueToken(AuthRequest req);
}

public class JwtWriter : IJwtWriter
{
    private readonly ILogger<JwtWriter> _logger;
    private readonly AuthConfig[] _configs;
    private readonly JwtSecurityTokenHandler _handler;

    public JwtWriter(ILogger<JwtWriter> logger, AuthConfig[] configs)
    {
        _logger = logger;
        _configs = configs;
        _handler = new JwtSecurityTokenHandler();
    }

    public AuthResponse? IssueToken(AuthRequest req)
    {
        var config = _configs.FirstOrDefault(x =>
            x.ClientId == req.ClientId &&
            x.ClientSecret == req.ClientSecret
        );

        if (config is null)
        {
            return null;
        }

        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var exp = now + config.TokenLifetime;

        var header = new JwtHeader
        (
            new SigningCredentials
            (
                new RsaSecurityKey(config.PrivateKey),
                SecurityAlgorithms.RsaSha256
            )
        );

        var payload = new JwtPayload
        {
            [JwtRegisteredClaimNames.Iss] = TokensApiConstants.Issuer,
            [JwtRegisteredClaimNames.Sub] = config.ClientId,
            [JwtRegisteredClaimNames.Iat] = now,
            [JwtRegisteredClaimNames.Exp] = exp,
            [TokensApiConstants.Scope] = string.Join(' ', config.Scopes)
        };

        var token = new JwtSecurityToken
        (
            header,
            payload
        );

        var serializedToken = _handler.WriteToken(token);
        if (serializedToken is null)
        {
            _logger.LogError("failed to write token {clientId} {clientSecret}", config.ClientId, config.ClientSecret);
            return null;
        }

        return new AuthResponse()
        {
            ExpiresAt = exp,
            Token = serializedToken,
            Scheme = TokensApiConstants.JwtAuthScheme,
        };
    }
}