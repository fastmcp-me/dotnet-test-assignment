using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using System.ComponentModel;
using WeatherMcpServer.Formatters;
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
        try
        {
            var currentWeather = await weatherService.GetCurrentWeather(city, countryCode);

            var description = currentWeather.RootElement.ToCurrentWeatherDescription();

            return $"Current weather in {city}{(countryCode is not null ? $", {countryCode}" : "")}: {description}";
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get current weather for {City}, {Country}", city, countryCode);
            return $"Could not retrieve weather for {city}{(countryCode is not null ? $", {countryCode}" : "")}.";
        }
    }

    [McpServerTool]
    [Description("Get weather forecast for a specified location.")]
    public async Task<string> GetWeatherForecast(
        [Description("The city name (e.g., 'London')")] string city,
        [Description("Optional country code (e.g., 'GB')")] string? countryCode = null)
    {
        try
        {
            var forecast = await weatherService.Get5Day3HourStepForecast(city, countryCode);

            var dailyDescriptions = forecast.RootElement.ToDailyForecastDescription(3);

            return $"Weather forecast for {city}{(countryCode is not null ? $", {countryCode}" : "")}:\n" + dailyDescriptions;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get weather forecast for {City}, {Country}", city, countryCode);
            return $"Could not retrieve weather forecast for {city}{(countryCode is not null ? $", {countryCode}" : "")}.";
        }
    }

    [McpServerTool]
    [Description("Get weather alerts/warnings for a location.")]
    public async Task<string> GetWeatherAlerts(
        [Description("The city name (e.g., 'London')")] string city,
        [Description("Optional country code (e.g., 'GB')")] string? countryCode = null)
    {
        try
        {
            var alerts = await weatherService.GetWeatherAlerts(city, countryCode);
            var description = alerts.RootElement.ToAlertsDescription();
            return $"Weather alerts for {city}{(countryCode is not null ? $", {countryCode}" : "")}:\n{description}";
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get weather alerts for {City}, {Country}", city, countryCode);
            return $"Could not retrieve weather alerts for {city}{(countryCode is not null ? $", {countryCode}" : "")}.";
        }
    }
}