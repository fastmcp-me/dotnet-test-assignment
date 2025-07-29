namespace WeatherMcpServer.Business.Models;

public record CurrentWeatherData
{
    public string City { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public double Temperature { get; init; }
    public double FeelsLike { get; init; }
    public string Description { get; init; } = string.Empty;
    public string MainCondition { get; init; } = string.Empty;
    public double Humidity { get; init; }
    public double WindSpeed { get; init; }
    public double Pressure { get; init; }
    public DateTime Timestamp { get; init; }
    public string Icon { get; init; } = string.Empty;
}