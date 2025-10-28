using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using StockTracker.Frontend.Constants;

namespace StockTracker.Frontend.Identity.Contracts.Implementation;

/// <summary>
/// Handler to ensure cookie credentials are automatically sent over with each request.
/// </summary>
public class CookieHandler : DelegatingHandler
{
    private readonly IWebAssemblyHostEnvironment _environment;
    private readonly IConfiguration _configInstance;

    public CookieHandler(IWebAssemblyHostEnvironment environment)
    {
        _environment = environment;
    }
    /// <summary>
    /// Main method to override for the handler.
    /// </summary>
    /// <param name="request">The original request.</param>
    /// <param name="cancellationToken">The token to handle cancellations.</param>
    /// <returns>The <see cref="HttpResponseMessage"/>.</returns>
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.SetBrowserRequestCredentials(
            request.RequestUri!.AbsoluteUri.Contains(FrontendConstants.GetTrackedCompaniesApiEndpointRoute)
                ? BrowserRequestCredentials.Omit
                : BrowserRequestCredentials.Include);
        request.SetBrowserRequestMode(BrowserRequestMode.Cors);
        return base.SendAsync(request, cancellationToken);
    }
}