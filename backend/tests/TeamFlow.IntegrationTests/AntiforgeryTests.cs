using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using TeamFlow.API.Controllers;

namespace TeamFlow.IntegrationTests;

public sealed class AntiforgeryTests
{
    [Fact]
    public async Task ControllerWritesRequireTheCookieAndRequestToken()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var factory = new WebApplicationFactory<AntiforgeryController>()
            .WithWebHostBuilder(builder => builder.ConfigureTestServices(services =>
            {
                services.Configure<KeyManagementOptions>(options => options.XmlRepository = new InMemoryKeyRepository());
                services.AddControllers().AddApplicationPart(typeof(AntiforgeryTestController).Assembly);
            }));

        using var browser = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });
        using var tokenResponse = await browser.GetAsync("/api/antiforgery/token", cancellationToken);
        Assert.Equal(HttpStatusCode.OK, tokenResponse.StatusCode);
        Assert.True(tokenResponse.Headers.CacheControl?.NoStore);
        var cookie = Assert.Single(tokenResponse.Headers.GetValues("Set-Cookie"));
        Assert.Contains("httponly", cookie, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("secure", cookie, StringComparison.OrdinalIgnoreCase);
        var token = await tokenResponse.Content.ReadFromJsonAsync<AntiforgeryTokenResponse>(cancellationToken);
        Assert.NotNull(token);

        using var missingHeader = await browser.PostAsync("/api/test/antiforgery", null, cancellationToken);
        Assert.Equal(HttpStatusCode.BadRequest, missingHeader.StatusCode);

        using var otherBrowser = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = browser.BaseAddress!
        });
        otherBrowser.DefaultRequestHeaders.Add("X-CSRF-TOKEN", token.RequestToken);
        using var missingCookie = await otherBrowser.PostAsync("/api/test/antiforgery", null, cancellationToken);
        Assert.Equal(HttpStatusCode.BadRequest, missingCookie.StatusCode);

        browser.DefaultRequestHeaders.Add("X-CSRF-TOKEN", token.RequestToken);
        using var accepted = await browser.PostAsync("/api/test/antiforgery", null, cancellationToken);
        Assert.Equal(HttpStatusCode.NoContent, accepted.StatusCode);
    }
}

[ApiController]
[Route("test/antiforgery")]
public sealed class AntiforgeryTestController : ControllerBase
{
    [HttpPost]
    public IActionResult Post() => NoContent();
}
