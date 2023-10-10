using System.Security.Cryptography;

using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace TokensApi.Utils;

public static class Extensions
{
    const string TokensApiPublicKey = nameof(TokensApiPublicKey);

    public static AuthenticationBuilder AddTokensApiAuthentication(this AuthenticationBuilder builder, IConfiguration config)
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

        builder.AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = TokensApiConstants.Issuer,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new RsaSecurityKey(rsa)
            };
            //options.MapInboundClaims = false;
            options.IncludeErrorDetails = true;
        });

        return builder;
    }

    public static void AddAuthorizationHeaderInput(this SwaggerGenOptions options)
    {
        options.AddSecurityDefinition(TokensApiConstants.JwtAuthScheme, new OpenApiSecurityScheme()
        {
            Name = HeaderNames.Authorization,
            Type = SecuritySchemeType.ApiKey,
            Scheme = TokensApiConstants.JwtAuthScheme,
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
                        Id = TokensApiConstants.JwtAuthScheme
                    }
                },
                new string[] { }
            }
        });
    }

    public static void AddBackendPolicy(this AuthorizationOptions options)
    {
        options.AddPolicy
        (
            Scopes.BackendScope,
            policy => policy.RequireClaim(TokensApiConstants.Scope, Scopes.BackendScope)
        );
    }
}