using System.Security.Cryptography;
using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace TokensApi.Utils;

public static class Extensions
{
    // expecting public key in base64
    const string TokensApiPublicKey = nameof(TokensApiPublicKey);

    public static IServiceCollection AddTokensApiAuthentication(this IServiceCollection services, IConfiguration config)
    {
        var pubKey = config.GetValue<string>(TokensApiPublicKey)
            ?? throw new ArgumentException($"{nameof(TokensApiPublicKey)} is missing");

        var bytes = Convert.FromBase64String(pubKey);

        var rsa = RSA.Create();
        try
        {
            rsa.ImportSubjectPublicKeyInfo(bytes, out _);
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