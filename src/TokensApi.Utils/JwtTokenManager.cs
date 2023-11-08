using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

using Microsoft.Extensions.Logging;

namespace TokensApi.Utils;

public interface IAuthManager
{
    AuthResponse? AuthResponse { get; }
    bool Init();
}

public class AuthManager : IAuthManager
{
    const int RetryTimeout = 4;
    const int RequestOffset = 15;

    private readonly ILogger<AuthManager> _logger;
    private readonly HttpClient _client;
    private readonly TokensApiConfig _config;
    private readonly object _lock;

    private Task? _refershTokenTask;
    private bool _errorLogged;

    public AuthManager(ILogger<AuthManager> logger, HttpClient client, TokensApiConfig config)
    {
        _logger = logger;
        _client = client;
        _config = config;
        _lock = new object();
    }

    public AuthResponse? AuthResponse { get; private set; }
    /// <summary>
    /// State flag which determines
    /// if it's necesssary to log an error
    /// </summary>
    private bool LogError => AuthResponse is null ||
        AuthResponse.ExpiresAt - DateTimeOffset.UtcNow.ToUnixTimeSeconds() <= 0 &&
        !_errorLogged;

    public static int SuccessDelay(AuthResponse resp)
    {
        var delay = (int)(resp.ExpiresAt - DateTimeOffset.UtcNow.ToUnixTimeSeconds() - RequestOffset) * 1000;
        return delay > 0 ? delay : RetryTimeout;
    }

    public bool Init()
    {
        lock (_lock)
        {
            if (_refershTokenTask is not null && AuthResponse is not null)
                return true;

            var ok = RefreshToken().GetAwaiter().GetResult();
            if (!ok)
                return false;

            _refershTokenTask = Loop();

            return true;
        }
    }

    private async Task Loop()
    {
        while (true)
        {
            var ok = await RefreshToken();
            if (ok && AuthResponse is not null)
            {
                _errorLogged = false;
                var delay = SuccessDelay(AuthResponse);
                await Task.Delay(delay);
                continue;
            }

            if (LogError)
            {
                _logger.LogError("not expired jwt token is missing");
                _errorLogged = true;
            }

            await Task.Delay(RetryTimeout * 1000);
        }
    }

    private async Task<bool> RefreshToken()
    {
        var msg = new HttpRequestMessage(HttpMethod.Post, _config.TokensApiUrl);

        var req = new AuthRequest()
        {
            ClientId = _config.ClientId,
            ClientSecret = _config.ClientSecret
        };
        var json = JsonSerializer.Serialize(req);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        msg.Content = content;

        var resp = await _client.SendAsync(msg);

        if (resp is null || !resp.IsSuccessStatusCode)
        {
            _logger.LogWarning("failed to authenticate for jwt token status: {status}", resp?.StatusCode);
            return false;
        }

        var authResp = await resp.Content.ReadFromJsonAsync<AuthResponse>();

        if (authResp is null)
        {
            _logger.LogWarning("failed to authenticate for jwt token, got null");
            return false;
        }

        AuthResponse = authResp;
        return true;
    }
}