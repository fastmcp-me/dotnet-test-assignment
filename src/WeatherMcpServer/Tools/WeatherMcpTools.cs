
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using System.ComponentModel;
using WeatherMcpServer.Application;
using WeatherMcpServer.Domain.LocationAggregate;
using WeatherMcpServer.Infrastructure.Exceptions;

namespace WeatherMcpServer.Tools;

public class WeatherMcpTools
{
    private readonly IWeatherService _weatherService;
    private readonly ILogger _logger;

    public WeatherMcpTools(
        IWeatherService weatherService,
        ILogger logger)
    {
        _weatherService = weatherService;
        _logger = logger;
    }

    [McpServerTool]
    [Description("Gets current weather conditions for the specified city.")]
    public async Task<string> GetCurrentWeather(
        [Description("The city name to get weather for")] string city,
        [Description("Optional: Country code (e.g., 'US', 'UK')")] string? countryCode = null)
    {
        try
        {
            var location = Location.Create(city, countryCode);
            var weather = await _weatherService.GetLocationWeatherAsync(location);

            return $"Current weather in {location}: {weather.CurrentWeather.Description}, " +
                    $"temp {weather.CurrentWeather.TemperatureCelsius}°C, " +
                    $"humidity {weather.CurrentWeather.Humidity}%";
        }
        catch (WeatherServiceException ex)
        {
            _logger.LogError(ex, "Exception occurred in WeatherService while fetching weather for city={City}, country={CountryCode}", city, countryCode);
            return $"WeatherService error: {ex.Message}";
        }
        catch (ExternalServiceException ex)
        {
            _logger.LogError(ex, "Network error while fetching weather for city={City}, country={CountryCode}", city, countryCode);
            return "Sorry, we are currently experiencing network issues while fetching weather data. Please try again later.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while fetching weather for city={City}, country={CountryCode}", city, countryCode);
            return "An unexpected error occurred. Please try again later.";
        }
    }

    [McpServerTool]
    [Description("Gets weather forecast for the specified city for the next given number of days.")]
    public async Task<string> GetWeatherForecast(
    [Description("The city name to get forecast for")] string city,
    [Description("Optional: Country code (e.g., 'US', 'UK')")] string? countryCode = null,
    [Description("Number of days for forecast (default 3)")] int days = 3)
    {
        try
        {
            var location = Location.Create(city, countryCode);
            var weather = await _weatherService.GetLocationWeatherAsync(location, days);

            if (weather.Forecasts == null || !weather.Forecasts.Any())
                return $"No forecast data available for {location}.";

            var forecastText = string.Join(Environment.NewLine, weather.Forecasts.Select(f =>
                $"{f.Date:yyyy-MM-dd}: {f.Description}, {f.TemperatureCelsius}°C"));

            return $"Weather forecast for {location}:\n{forecastText}";
        }
        catch (WeatherServiceException ex)
        {
            _logger.LogError(ex, "Exception occurred in WeatherService while fetching forecast for city={City}, country={CountryCode}", city, countryCode);
            return $"WeatherService error: {ex.Message}";
        }
        catch (ExternalServiceException ex)
        {
            _logger.LogError(ex, "Network error while fetching forecast for city={City}, country={CountryCode}", city, countryCode);
            return "Sorry, we are currently experiencing network issues while fetching forecast data. Please try again later.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while fetching forecast for city={City}, country={CountryCode}", city, countryCode);
            return "An unexpected error occurred. Please try again later.";
        }
    }

    [McpServerTool]
    [Description("Gets active weather alerts for the specified city.")]
    public async Task<string> GetWeatherAlerts(
    [Description("The city name to get alerts for")] string city,
    [Description("Optional: Country code (e.g., 'US', 'UK')")] string? countryCode = null)
    {
        try
        {
            var location = Location.Create(city, countryCode);
            var weather = await _weatherService.GetLocationWeatherAsync(location);

            if (weather.Alerts == null || !weather.Alerts.Any())
                return $"No active weather alerts for {location}.";

            var alertsText = string.Join(Environment.NewLine, weather.Alerts.Select(a =>
                $"{a.Severity} - {a.Title} ({a.EffectiveFrom:yyyy-MM-dd HH:mm} to {a.EffectiveTo:yyyy-MM-dd HH:mm}): {a.Description}"
            ));

            return $"Active weather alerts for {location}:\n{alertsText}";
        }
        catch (WeatherServiceException ex)
        {
            _logger.LogError(ex, "Exception occurred in WeatherService while fetching alerts for city={City}, country={CountryCode}", city, countryCode);
            return $"WeatherService error: {ex.Message}";
        }
        catch (ExternalServiceException ex)
        {
            _logger.LogError(ex, "Network error while fetching alerts for city={City}, country={CountryCode}", city, countryCode);
            return "Sorry, we are currently experiencing network issues while fetching weather alerts. Please try again later.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while fetching alerts for city={City}, country={CountryCode}", city, countryCode);
            return "An unexpected error occurred. Please try again later.";
        }
    }
}
