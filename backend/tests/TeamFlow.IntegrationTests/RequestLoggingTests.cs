using System.Collections.Concurrent;
using System.Net;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TeamFlow.API;
using TeamFlow.API.Controllers;

namespace TeamFlow.IntegrationTests;

public sealed class RequestLoggingTests
{
    [Theory]
    [InlineData("/api/test/logging/private-route-value", HttpStatusCode.NoContent, LogLevel.Information, "test/logging/{value}")]
    [InlineData("/api/test/logging/private-route-value/failure", HttpStatusCode.InternalServerError, LogLevel.Error, "test/logging/{value}/failure")]
    [InlineData("/api/private-route-value", HttpStatusCode.NotFound, LogLevel.Warning, "(no route)")]
    public async Task ResponseIdCorrelatesWithSanitizedRequestLogs(
        string path, HttpStatusCode status, LogLevel level, string route)
    {
        using var logs = new RequestLogProvider();
        await using var factory = new WebApplicationFactory<AntiforgeryController>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureLogging(logging => logging.AddProvider(logs));
                builder.ConfigureTestServices(services =>
                {
                    services.Configure<KeyManagementOptions>(options => options.XmlRepository = new InMemoryKeyRepository());
                    services.AddControllers().AddApplicationPart(typeof(RequestLoggingTestController).Assembly);
                });
            });
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });
        client.DefaultRequestHeaders.Add("X-Request-Id", "client-supplied-id");

        using var response = await client.GetAsync(path + "?token=private-query-value", TestContext.Current.CancellationToken);

        Assert.Equal(status, response.StatusCode);
        var id = Assert.Single(response.Headers.GetValues("X-Request-Id"));
        Assert.False(string.IsNullOrWhiteSpace(id));
        Assert.NotEqual("client-supplied-id", id);

        var request = Assert.Single(logs.Entries, entry => entry.Category == typeof(RequestLoggingMiddleware).FullName);
        Assert.Equal(level, request.Level);
        Assert.Equal(id, request.RequestId);
        Assert.Equal(id, request.Properties["RequestId"]);
        Assert.Equal("GET", request.Properties["Method"]);
        Assert.Equal(route, request.Properties["Route"]);
        Assert.Equal((int)status, request.Properties["StatusCode"]);
        Assert.True(Assert.IsType<double>(request.Properties["ElapsedMs"]) >= 0);

        if (status == HttpStatusCode.InternalServerError)
        {
            var failure = Assert.Single(logs.Entries, entry => entry.Category == typeof(GlobalExceptionHandler).FullName);
            Assert.Equal(id, failure.RequestId);
            Assert.Equal(id, failure.Properties["RequestId"]);
            Assert.Equal(LogLevel.Error, failure.Level);
            Assert.Equal(typeof(InvalidOperationException).FullName, failure.Properties["ExceptionType"]);
        }

        Assert.All(logs.Entries, entry => Assert.DoesNotContain("private-", entry.Message));
    }

    private sealed record LogEntry(
        string Category, LogLevel Level, string? RequestId, string Message, IReadOnlyDictionary<string, object?> Properties);

    private sealed class RequestLogProvider : ILoggerProvider, ISupportExternalScope
    {
        private IExternalScopeProvider scopes = new LoggerExternalScopeProvider();
        public ConcurrentQueue<LogEntry> Entries { get; } = new();

        public ILogger CreateLogger(string categoryName) => new RequestLogger(this, categoryName);
        public void SetScopeProvider(IExternalScopeProvider scopeProvider) => scopes = scopeProvider;
        public void Dispose() { }

        private sealed class RequestLogger(RequestLogProvider provider, string category) : ILogger
        {
            public IDisposable? BeginScope<TState>(TState state) where TState : notnull => provider.scopes.Push(state);

            public bool IsEnabled(LogLevel logLevel) =>
                category == typeof(RequestLoggingMiddleware).FullName || category == typeof(GlobalExceptionHandler).FullName;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
                Func<TState, Exception?, string> formatter)
            {
                if (!IsEnabled(logLevel) || state is not IEnumerable<KeyValuePair<string, object?>> properties)
                {
                    return;
                }

                var scopeProperties = new Dictionary<string, object?>();
                provider.scopes.ForEachScope((scope, values) =>
                {
                    if (scope is IEnumerable<KeyValuePair<string, object?>> fields)
                    {
                        foreach (var field in fields)
                        {
                            values[field.Key] = field.Value;
                        }
                    }
                }, scopeProperties);

                provider.Entries.Enqueue(new LogEntry(category, logLevel, scopeProperties.GetValueOrDefault("RequestId") as string,
                    formatter(state, exception), properties.ToDictionary()));
            }
        }
    }
}

[ApiController]
[Route("test/logging/{value}")]
public sealed class RequestLoggingTestController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => NoContent();

    [HttpGet("failure")]
    public IActionResult Fail() => throw new InvalidOperationException("private-exception-message");
}
