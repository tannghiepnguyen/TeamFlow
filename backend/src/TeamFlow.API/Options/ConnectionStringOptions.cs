namespace TeamFlow.API.Options;

internal sealed class ConnectionStringOptions
{
    public string? TeamFlow { get; set; }
    public string? DefaultConnection { get; set; }

    public string? ConnectionString => string.IsNullOrWhiteSpace(TeamFlow) ? DefaultConnection : TeamFlow;
}
