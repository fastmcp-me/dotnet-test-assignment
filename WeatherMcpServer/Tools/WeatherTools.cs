using System.ComponentModel;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using WeatherMcpServer.Services;

namespace WeatherMcpServer.Tools;

public class WeatherTools(
    OpenWeatherService weatherService,
    ILogger<WeatherTools> logger)
{
    [McpServerTool]
    [Description("Describes random weather in the provided city.")]
    public string GetCityWeather(
        [Description("Name of the city to return weather for")] string city)
    {
        // Read the environment variable during tool execution.
        // Alternatively, this could be read during startup and passed via IOptions dependency injection
        var weather = Environment.GetEnvironmentVariable("WEATHER_CHOICES");
        if (string.IsNullOrWhiteSpace(weather))
        {
            weather = "balmy,rainy,stormy";
        }

        var weatherChoices = weather.Split(",");
        var selectedWeatherIndex =  Random.Shared.Next(0, weatherChoices.Length);

        return $"The weather in {city} is {weatherChoices[selectedWeatherIndex]}.";
    }

    [McpServerTool]
    [Description("Get current weather for a specified location.")]
    public async Task<string> GetCurrentWeather(
        [Description("The city name (e.g., 'London')")] string city,
        [Description("Optional country code (e.g., 'GB')")] string? countryCode = null)
    {
        throw new NotImplementedException();
    }

    [McpServerTool]
    [Description("Get weather forecast for a specified location.")]
    public async Task<string> GetWeatherForecast(
        [Description("The city name (e.g., 'London')")] string city,
        [Description("Optional country code (e.g., 'GB')")] string? countryCode = null,
        [Description("Number of days to include in the forecast (default is 3)")] int days = 3)
    {
        throw new NotImplementedException();
    }

    [McpServerTool]
    [Description("Get weather alerts/warnings for a location.")]
    public async Task<string> GetWeatherAlerts(
        [Description("The city name (e.g., 'London')")] string city,
        [Description("Optional country code (e.g., 'GB')")] string? countryCode = null)
    {
        throw new NotImplementedException();
    }
}