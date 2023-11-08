using System.Net.Http.Headers;

namespace TokensApi.Utils;

public class TokensApiHandler : DelegatingHandler
{
    private readonly IAuthManager _manager;
    
    public TokensApiHandler(IAuthManager manager)
    {
        _manager = manager;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token)
    {
        if (_manager.AuthResponse is not null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue
            (
                _manager.AuthResponse.Scheme,
                _manager.AuthResponse.Token
            );
        }

        return base.SendAsync(request, token);
    }
}