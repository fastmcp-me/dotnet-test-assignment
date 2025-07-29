using System.ComponentModel;
using System.Text;
using MediatR;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using WeatherMcpServer.Business.Infrastructure.Exceptions;
using WeatherMcpServer.Business.Models;
using WeatherMcpServer.Business.Queries.GetCurrentWeather;
using WeatherMcpServer.Business.Queries.GetWeatherAlerts;
using WeatherMcpServer.Business.Queries.GetWeatherForecast;

namespace WeatherMcpServer.Tools;

public class WeatherTools
{
    private readonly IMediator _mediator;
    private readonly ILogger<WeatherTools> _logger;

    public WeatherTools(IMediator mediator, ILogger<WeatherTools> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [McpServerTool]
    [Description("Gets current weather conditions for the specified city.")]
    public async Task<string> GetCurrentWeather(
        [Description("The city name to get weather for")] string city,
        [Description("Optional: Country code (e.g., 'US', 'UK')")] string? countryCode = null)
    {
        try
        {
            _logger.LogInformation("Getting current weather for city: {City}, Country: {CountryCode}", city, countryCode);
            
            var query = new GetCurrentWeatherQuery(city, countryCode);
            var weather = await _mediator.Send(query);
            
            var result = FormatCurrentWeatherResponse(weather);
            _logger.LogInformation("Successfully retrieved current weather for city: {City}", city);
            
            return result;
        }
        catch (FluentValidation.ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed for city: {City}", city);
            var errors = string.Join("; ", ex.Errors.Select(e => e.ErrorMessage));
            throw new WeatherServiceException($"Invalid request: {errors}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current weather for city: {City}", city);
            throw new WeatherServiceException($"Failed to get weather for {city}. Please try again later.");
        }
    }

    [McpServerTool]
    [Description("Gets weather forecast for the specified city.")]
    public async Task<string> GetWeatherForecast(
        [Description("The city name to get weather forecast for")] string city,
        [Description("Optional: Country code (e.g., 'US', 'UK')")] string? countryCode = null,
        [Description("Number of days to forecast (1-5)")] int days = 3)
    {
        try
        {
            _logger.LogInformation("Getting weather forecast for city: {City}, Country: {CountryCode}, Days: {Days}", 
                city, countryCode, days);
            
            var query = new GetWeatherForecastQuery(city, countryCode, days);
            var forecast = await _mediator.Send(query);
            
            var result = FormatWeatherForecastResponse(forecast);
            _logger.LogInformation("Successfully retrieved weather forecast for city: {City}", city);
            
            return result;
        }
        catch (FluentValidation.ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed for city: {City}", city);
            var errors = string.Join("; ", ex.Errors.Select(e => e.ErrorMessage));
            throw new WeatherServiceException($"Invalid request: {errors}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting weather forecast for city: {City}", city);
            throw new WeatherServiceException($"Failed to get weather forecast for {city}. Please try again later.");
        }
    }

    [McpServerTool]
    [Description("Gets weather alerts/warnings for a location (requires paid API subscription).")]
    public async Task<string> GetWeatherAlerts(
        [Description("The city name to get weather alerts for")] string city,
        [Description("Optional: Country code (e.g., 'US', 'UK')")] string? countryCode = null)
    {
        try
        {
            _logger.LogInformation("Getting weather alerts for city: {City}, Country: {CountryCode}", city, countryCode);
            
            var query = new GetWeatherAlertsQuery(city, countryCode);
            var alerts = await _mediator.Send(query);
            
            var result = FormatWeatherAlertsResponse(alerts);
            _logger.LogInformation("Successfully retrieved weather alerts for city: {City}", city);
            
            return result;
        }
        catch (FluentValidation.ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed for city: {City}", city);
            var errors = string.Join("; ", ex.Errors.Select(e => e.ErrorMessage));
            throw new WeatherServiceException($"Invalid request: {errors}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting weather alerts for city: {City}", city);
            throw new WeatherServiceException($"Failed to get weather alerts for {city}. Please try again later.");
        }
    }

    private static string FormatCurrentWeatherResponse(CurrentWeatherData weather)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"📍 Current Weather in {weather.City}, {weather.Country}");
        sb.AppendLine($"🌡️ Temperature: {weather.Temperature}°C (feels like {weather.FeelsLike}°C)");
        sb.AppendLine($"☁️ Condition: {weather.MainCondition} - {weather.Description}");
        sb.AppendLine($"💧 Humidity: {weather.Humidity}%");
        sb.AppendLine($"💨 Wind Speed: {weather.WindSpeed} m/s");
        sb.AppendLine($"🔵 Pressure: {weather.Pressure} hPa");
        sb.AppendLine($"⏰ Last Updated: {weather.Timestamp:yyyy-MM-dd HH:mm:ss}");
        
        return sb.ToString();
    }

    private static string FormatWeatherForecastResponse(WeatherForecastData forecast)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"📅 Weather Forecast for {forecast.City}, {forecast.Country}");
        sb.AppendLine();

        foreach (var day in forecast.DailyForecasts)
        {
            sb.AppendLine($"📆 {day.Date:dddd, MMMM d}");
            sb.AppendLine($"   🌡️ Temperature: {day.Temperature}°C (Min: {day.TemperatureMin}°C, Max: {day.TemperatureMax}°C)");
            sb.AppendLine($"   ☁️ Condition: {day.MainCondition} - {day.Description}");
            sb.AppendLine($"   💧 Humidity: {day.Humidity}% | Precipitation: {day.PrecipitationProbability}%");
            sb.AppendLine($"   💨 Wind Speed: {day.WindSpeed} m/s");
            sb.AppendLine();
        }

        return sb.ToString();
    }

    private static string FormatWeatherAlertsResponse(WeatherAlertsData alerts)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"⚠️ Weather Alerts for {alerts.City}, {alerts.Country}");
        sb.AppendLine();

        if (alerts.Alerts.Count == 0)
        {
            sb.AppendLine("✅ No active weather alerts for this location.");
            sb.AppendLine();
            sb.AppendLine("ℹ️ Note: Weather alerts require a paid API subscription.");
        }
        else
        {
            foreach (var alert in alerts.Alerts)
            {
                var severityIcon = alert.Severity switch
                {
                    AlertSeverity.Extreme => "🔴",
                    AlertSeverity.Severe => "🟠",
                    AlertSeverity.Moderate => "🟡",
                    AlertSeverity.Minor => "🟢",
                    _ => "⚪"
                };

                sb.AppendLine($"{severityIcon} {alert.Title}");
                sb.AppendLine($"   Severity: {alert.Severity}");
                sb.AppendLine($"   Event: {alert.Event}");
                sb.AppendLine($"   Duration: {alert.StartTime:yyyy-MM-dd HH:mm} - {alert.EndTime:yyyy-MM-dd HH:mm}");
                sb.AppendLine($"   Description: {alert.Description}");
                sb.AppendLine();
            }
        }

        return sb.ToString();
    }
}