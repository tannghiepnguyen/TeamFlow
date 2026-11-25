using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TeamFlow.API.Controllers;

[ApiController]
[AllowAnonymous]
[Route("antiforgery")]
public sealed class AntiforgeryController(IAntiforgery antiforgery) : ControllerBase
{
    [HttpGet("token")]
    public AntiforgeryTokenResponse GetToken()
    {
        var tokens = antiforgery.GetAndStoreTokens(HttpContext);
        return new AntiforgeryTokenResponse(tokens.RequestToken!);
    }
}

public sealed record AntiforgeryTokenResponse(string RequestToken);
