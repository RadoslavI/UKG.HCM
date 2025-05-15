using System.Net.Http.Headers;
using UKG.HCM.UI.Services.Interfaces;

namespace UKG.HCM.UI.Services;

public class HttpClientAuthorizationDelegatingHandler : DelegatingHandler
{
    private readonly IAuthService _authService;

    public HttpClientAuthorizationDelegatingHandler(IAuthService authService)
    {
        _authService = authService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = _authService.GetToken();
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
