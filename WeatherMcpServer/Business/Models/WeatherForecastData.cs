namespace WeatherMcpServer.Business.Models;

public record WeatherForecastData
{
    public string City { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public List<DailyForecast> DailyForecasts { get; init; } = new();
}

public record DailyForecast
{
    public DateTime Date { get; init; }
    public double TemperatureMin { get; init; }
    public double TemperatureMax { get; init; }
    public double Temperature { get; init; }
    public string Description { get; init; } = string.Empty;
    public string MainCondition { get; init; } = string.Empty;
    public double Humidity { get; init; }
    public double WindSpeed { get; init; }
    public double PrecipitationProbability { get; init; }
    public string Icon { get; init; } = string.Empty;
}