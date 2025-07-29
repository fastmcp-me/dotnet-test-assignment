namespace WeatherMcpServer.Integrations.OpenWeatherMap.Models;

public class OpenWeatherMapConfiguration
{
    public const string SectionName = "OpenWeatherMap";
    
    public string ApiKey { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://api.openweathermap.org/data/2.5";
    public string Units { get; set; } = "metric"; // metric, imperial, standard
    public int TimeoutSeconds { get; set; } = 30;
}