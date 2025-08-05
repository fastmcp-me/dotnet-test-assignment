using System.ComponentModel;
using ModelContextProtocol.Server;
using WeatherMcpServer.Formatters;
using WeatherMcpServer.Services;


public class WeatherTools
{
    private readonly IWeatherService _weatherService;

    public WeatherTools(IWeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    [McpServerTool]
    [Description("Gets current weather conditions for the specified city.")]
    public async Task<string> GetCurrentWeather(
        [Description("The city name to get weather for")]
        string city,
        [Description("Optional: Country code (e.g., 'US', 'UK')")]
        string? countryCode = null)
    {
        var location = await _weatherService.GetLocation(city + (string.IsNullOrEmpty(countryCode) ? "" : $",{countryCode}"));

        var weather = await _weatherService.GetWeather(location);
        return WeatherFormatter.FormatCurrentWeather(weather.Current, location.Name);
    }

    [McpServerTool]
    [Description("Gets forecast for 8 days for the specified city.")]
    public async Task<string> GetForecast(
        [Description("The city name to get weather for")]
        string city,
        [Description("Optional: Country code (e.g., 'US', 'UK')")]
        string? countryCode = null)
    {
        var location = await _weatherService.GetLocation(city + (string.IsNullOrEmpty(countryCode) ? "" : $",{countryCode}"));

        var weather = await _weatherService.GetWeather(location);

        return WeatherFormatter.FormatForecast(weather.Daily, location.Name);

    }
    
    [McpServerTool]
    [Description("Gets weather alerts for the specified city.")]
    public async Task<string> GetAlerts(
        [Description("The city name to get weather for")]
        string city,
        [Description("Optional: Country code (e.g., 'US', 'UK')")]
        string? countryCode = null)
    {
        var location = await _weatherService.GetLocation(city + (string.IsNullOrEmpty(countryCode) ? "" : $",{countryCode}"));

        var weather = await _weatherService.GetWeather(location);

        return WeatherFormatter.FormatAlerts(weather.Alerts, location.Name);

    }
}
