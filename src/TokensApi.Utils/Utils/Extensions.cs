using System.Security.Cryptography;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace TokensApi.Utils;

public static class Extensions
{
    // expecting public key in base64
    const string TokensApiPrivateKey = nameof(TokensApiPrivateKey);

    public static IServiceCollection AddTokensApiAuthentication(this IServiceCollection services, IConfiguration config)
    {
        var pKey = config.GetValue<string>(TokensApiPrivateKey)
            ?? throw new ArgumentException($"{nameof(TokensApiPrivateKey)} is missing");

        var rsa = RSA.Create();
        try
        {
            rsa.ImportFromPem(pKey);
        }
        catch (ArgumentException ex)
        {
            Console.Error.WriteLine("invalid public key is provided");
            throw ex;
        }

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = Constants.Issuer,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new RsaSecurityKey(rsa)
                };
                options.MapInboundClaims = false;
            });
        
        services.AddAuthorization();

        return services;
    }
}