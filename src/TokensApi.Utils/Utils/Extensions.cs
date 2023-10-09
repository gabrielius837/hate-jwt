using System.Net.Http.Headers;
using System.Security.Cryptography;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

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

    public static void AddAuthorizationHeaderInput(this SwaggerGenOptions options)
    {
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
        {
            Name = HeaderNames.Authorization,
            Type = SecuritySchemeType.ApiKey,
            Scheme = Constants.JwtAuthScheme,
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter authorization header value in the following format:\n'Bearer {your_jwt_token}'",
        });
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = Constants.JwtAuthScheme
                    }
                },
                new string[] { }
            }
        });
    }
}