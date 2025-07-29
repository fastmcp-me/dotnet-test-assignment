namespace WeatherMcpServer.Business.Models;

public record WeatherAlertsData
{
    public string City { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public List<WeatherAlert> Alerts { get; init; } = new();
}

public record WeatherAlert
{
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public AlertSeverity Severity { get; init; }
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }
    public string Event { get; init; } = string.Empty;
}

public enum AlertSeverity
{
    Unknown,
    Minor,
    Moderate,
    Severe,
    Extreme
}