namespace TokensApi;

public static class TokensApiExtensions
{
    public static IServiceCollection AddAuthenticationConfig(this IServiceCollection services, IConfiguration config)
    {
        var authSettings = config.GetSection(nameof(AuthenticationSettings)).Get<AuthenticationSettings>()
            ?? throw new ArgumentException($"{nameof(AuthenticationSettings)} are missing");

        var authConfig = authSettings.TryBuild()
            ?? throw new ArgumentException($"failed to build {nameof(AuthenticationConfig)}");

        services.AddSingleton(authConfig);

        return services;
    }
}