using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;

namespace TeamFlow.API;

public sealed class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    public const string RequestIdHeader = "X-Request-Id";

    public async Task InvokeAsync(HttpContext context)
    {
        var requestId = context.TraceIdentifier;
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[RequestIdHeader] = requestId;
            return Task.CompletedTask;
        });

        using var scope = logger.BeginScope("RequestId: {RequestId}", requestId);
        var started = Stopwatch.GetTimestamp();
        var completed = false;

        try
        {
            await next(context);
            completed = true;
        }
        finally
        {
            var endpoint = context.GetEndpoint() ?? context.Features.Get<IExceptionHandlerFeature>()?.Endpoint;
            var route = (endpoint as RouteEndpoint)?.RoutePattern.RawText ?? "(no route)";
            int? status = completed || context.Response.HasStarted ? context.Response.StatusCode : null;
            var level = !completed || status >= 500 ? LogLevel.Error
                : status >= 400 ? LogLevel.Warning
                : LogLevel.Information;

            logger.Log(level, "HTTP {Method} {Route} finished with status {StatusCode} in {ElapsedMs:F1} ms (request {RequestId})",
                context.Request.Method, route, status, Stopwatch.GetElapsedTime(started).TotalMilliseconds, requestId);
        }
    }
}
