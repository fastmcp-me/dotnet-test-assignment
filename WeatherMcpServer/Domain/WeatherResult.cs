namespace WeatherMcpServer.Domain;

public record WeatherResult
{
    public string ProviderName { get; init; } = default!;
    public string Location { get; init; } = default!;
    public DateTimeOffset Timestamp { get; init; }
    public string Description { get; init; } = default!;
    public double TemperatureC { get; init; }
    public bool IsFallback { get; init; } = false;
}
