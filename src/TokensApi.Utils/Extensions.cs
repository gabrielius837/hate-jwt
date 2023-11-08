using System.Security.Cryptography;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

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
        rsa.ImportSubjectPublicKeyInfo(bytes, out _);

        builder.AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = TokensApiConstants.Issuer,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new RsaSecurityKey(rsa)
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
                Array.Empty<string>()
            }
        });
    }

    public static void AddBackendPolicy(this AuthorizationOptions options)
    {
        options.AddPolicy
        (
            Scope.BackendScope,
            policy => policy.RequireClaim(TokensApiConstants.Scope, Scope.BackendScope)
        );
    }

    public static IServiceCollection AddTokensApiClient(this IServiceCollection services, IConfiguration config)
    {
        // get config
        // get logger
        // get token manager
        // get client
        services.AddHttpClient<AuthManager>();
        var tokenApiSetting = config.GetRequiredSection(nameof(TokensApiSetting)).Get<TokensApiSetting>();
        var tokensApiConfig = tokenApiSetting.TryBuild() ?? throw new ArgumentException($"failed to build {nameof(TokensApiConfig)}");

        using var provider = services.BuildServiceProvider().CreateScope();

        var logger = provider.ServiceProvider.GetRequiredService<ILogger<AuthManager>>();
        var client = provider.ServiceProvider.GetRequiredService<HttpClient>();
        
        var authManager = new AuthManager(logger, client, tokensApiConfig);
        if (!authManager.Init())
        {
            throw new ArgumentException("failed to initialize auth manager for tokens-api");
        }

        var handler = new TokensApiHandler(authManager);

        services.AddSingleton(handler);
        services.AddSingleton<IAuthManager>(authManager);
        services.AddHttpClient<HttpClient>()
            .AddHttpMessageHandler<TokensApiHandler>();

        return services;
    }

    /// <summary>
    /// Reads up a setting from <see cref="IConfiguration"/>
    /// which is converted and persisted as a singleton.
    /// </summary>
    /// <typeparam name="T">Type which is read from settings.</typeparam>
    /// <typeparam name="K">Type which is persisted.</typeparam>
    /// <param name="services">Service collection.</param>
    /// <param name="config">The configuration.</param>
    /// <param name="section">Key which locates setting</param>
    /// <param name="tryBuild">Function ref which transforms and validates input</param>
    /// <returns>Service collection to allow chaining</returns>
    /// <exception cref="ConfigurationException">Thrown whenever setting is missing or it failed to be transformed.</exception>
    public static IServiceCollection AddConfig<T, K>(this IServiceCollection services, IConfiguration config, string section, Func<T, K?> tryBuild)
        where T : class
        where K : class
    {
        var setting = config.GetSection(section).Get<T>()
            ?? throw new ConfigurationException($"{nameof(T)} is null under section: {section}");
        
        var result = tryBuild(setting)
            ?? throw new ConfigurationException($"failed to build {nameof(K)} from {nameof(T)} under section: {section}");

        services.AddSingleton(result);

        return services;
    }

    /// <summary>
    /// Reads up a setting from <see cref="IConfiguration"/>
    /// which is persisted as a singleton.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="services"></param>
    /// <param name="config"></param>
    /// <param name="section"></param>
    /// <param name="validator"></param>
    /// <returns></returns>
    /// <exception cref="ConfigurationException"></exception>
    public static IServiceCollection AddConfig<T>(this IServiceCollection services, IConfiguration config, string section, Func<T?, bool> validator)
        where T : class
    {
        var setting = config.GetSection(section).Get<T>()
            ?? throw new ConfigurationException($"{nameof(T)} is null under section: {section}");
        
        var valid = validator(setting);
        if (!valid)
        {
            throw new ConfigurationException($"failed to validate {nameof(T)} under section: {section}");
        }

        services.AddSingleton(setting);

        return services;
    }
}