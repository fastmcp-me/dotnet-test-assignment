using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using System.ComponentModel;
using WeatherMcpServer.Services;

namespace WeatherMcpServer.Tools;

public class WeatherTools(
    OpenWeatherService weatherService,
    ILogger<WeatherTools> logger)
{
    [McpServerTool]
    [Description("Get current weather for a specified location.")]
    public async Task<string> GetCurrentWeather(
        [Description("The city name (e.g., 'London')")] string city,
        [Description("Optional country code (e.g., 'GB')")] string? countryCode = null)
    {
        if (string.IsNullOrWhiteSpace(city))
            return "City name must be provided.";

        logger.LogDebug("Calling GetCurrentWeather for city {City}, country {Country}", city, countryCode);
        try
        {
            var description = await weatherService.GetCurrentWeatherDescription(city, countryCode);
            return $"Current weather in {city}{(countryCode is not null ? $", {countryCode}" : "")}: {description}";
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid argument for GetCurrentWeather: {Message}", ex.Message);
            return ex.Message;
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "API error for GetCurrentWeather: {Message}", ex.Message);
            return $"Could not retrieve current weather for {city}{(countryCode is not null ? $", {countryCode}" : "")}. {ex.Message}";
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get current weather for {City}, {Country}", city, countryCode);
            return $"Could not retrieve current weather for {city}{(countryCode is not null ? $", {countryCode}" : "")}. {ex.Message}";
        }
    }

    [McpServerTool]
    [Description("Get weather forecast for a specified location.")]
    public async Task<string> GetWeatherForecast(
        [Description("The city name (e.g., 'London')")] string city,
        [Description("Optional country code (e.g., 'GB')")] string? countryCode = null)
    {
        if (string.IsNullOrWhiteSpace(city))
            return "City name must be provided.";

        logger.LogDebug("Calling GetWeatherForecast for city {City}, country {Country}", city, countryCode);
        try
        {
            var dailyDescriptions = await weatherService.Get5Day3HourStepForecast(city, countryCode);
            return $"Weather forecast for {city}{(countryCode is not null ? $", {countryCode}" : "")}:\n" + dailyDescriptions;
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid argument for GetWeatherForecast: {Message}", ex.Message);
            return ex.Message;
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "API error for GetWeatherForecast: {Message}", ex.Message);
            return $"Could not retrieve weather forecast for {city}{(countryCode is not null ? $", {countryCode}" : "")}. {ex.Message}";
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get weather forecast for {City}, {Country}", city, countryCode);
            return $"Could not retrieve weather forecast for {city}{(countryCode is not null ? $", {countryCode}" : "")}. {ex.Message}";
        }
    }

    [McpServerTool]
    [Description("Get weather alerts/warnings for a location.")]
    public async Task<string> GetWeatherAlerts(
        [Description("The city name (e.g., 'London')")] string city,
        [Description("Optional country code (e.g., 'GB')")] string? countryCode = null)
    {
        if (string.IsNullOrWhiteSpace(city))
            return "City name must be provided.";

        logger.LogDebug("Calling GetWeatherAlerts for city {City}, country {Country}", city, countryCode);
        try
        {
            var description = await weatherService.GetWeatherAlertsDescription(city, countryCode);
            return $"Weather alerts for {city}{(countryCode is not null ? $", {countryCode}" : "")}:\n{description}";
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid argument for GetWeatherAlerts: {Message}", ex.Message);
            return ex.Message;
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "API error for GetWeatherAlerts: {Message}", ex.Message);
            return $"Could not retrieve weather alerts for {city}{(countryCode is not null ? $", {countryCode}" : "")}. {ex.Message}";
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get weather alerts for {City}, {Country}", city, countryCode);
            return $"Could not retrieve weather alerts for {city}{(countryCode is not null ? $", {countryCode}" : "")}. {ex.Message}";
        }
    }
}