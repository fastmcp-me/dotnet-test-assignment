using System.ComponentModel;
using MediatR;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using WeatherMcpServer.Business.Infrastructure.Exceptions;
using WeatherMcpServer.Business.Infrastructure.Formatters;
using WeatherMcpServer.Business.Queries.GetCurrentWeather;
using WeatherMcpServer.Business.Queries.GetWeatherAlerts;
using WeatherMcpServer.Business.Queries.GetWeatherForecast;

namespace WeatherMcpServer.Tools;

public class WeatherTools
{
    private readonly IMediator _mediator;
    private readonly ILogger<WeatherTools> _logger;
    private readonly IWeatherResponseFormatter _formatter;

    public WeatherTools(
        IMediator mediator, 
        ILogger<WeatherTools> logger,
        IWeatherResponseFormatter formatter)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
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
            
            var result = _formatter.FormatCurrentWeather(weather);
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
            
            var result = _formatter.FormatWeatherForecast(forecast);
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
            
            var result = _formatter.FormatWeatherAlerts(alerts);
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
}