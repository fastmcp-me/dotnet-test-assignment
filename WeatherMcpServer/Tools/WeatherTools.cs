using System.ComponentModel;
using ModelContextProtocol.Server;
using MediatR;
using Serilog;
using System.Diagnostics;
using WeatherMcpServer.Commands;

namespace WeatherMcpServer.Tools;

/// <summary>
/// MCP tools for weather-related operations
/// </summary>
public class WeatherTools
{
    private readonly IMediator _mediator;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the WeatherTools
    /// </summary>
    /// <param name="mediator">MediatR mediator for command handling</param>
    /// <param name="logger">Serilog logger for structured logging</param>
    public WeatherTools(IMediator mediator, ILogger logger)
    {
        _mediator = mediator;
        _logger = logger.ForContext<WeatherTools>();
    }

    /// <summary>
    /// Gets current weather conditions for the specified city
    /// </summary>
    /// <param name="city">The city name to get weather for</param>
    /// <param name="countryCode">Optional country code to narrow the search</param>
    /// <param name="units">Temperature units (metric, imperial, kelvin)</param>
    /// <param name="language">Language code for weather descriptions</param>
    /// <returns>Formatted current weather information</returns>
    [McpServerTool]
    [Description("Gets current weather conditions for the specified city.")]
    public async Task<string> GetCurrentWeather(
        [Description("The city name to get weather for")] string city,
        [Description("Optional: Country code (e.g., 'US', 'UK')")] string? countryCode = null,
        [Description("Temperature units: 'metric' (Celsius), 'imperial' (Fahrenheit), 'kelvin'")] string units = "metric",
        [Description("Language code for weather descriptions (e.g., 'en', 'ru', 'es')")] string language = "en")
    {
        using var activity = new Activity("McpTool_GetCurrentWeather").Start();
        activity?.SetTag("mcp.tool", "GetCurrentWeather");
        activity?.SetTag("weather.city", city);
        activity?.SetTag("weather.country_code", countryCode);
        
        _logger.Information("MCP Tool called: GetCurrentWeather for {City}, {CountryCode}", city, countryCode);
        
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            var command = new GetCurrentWeatherCommand(city, countryCode, units, language);
            var result = await _mediator.Send(command);
            
            _logger.Information("MCP Tool GetCurrentWeather completed for {City} in {ElapsedMs}ms", 
                city, stopwatch.ElapsedMilliseconds);
            
            activity?.SetTag("weather.success", true);
            activity?.SetTag("weather.duration_ms", stopwatch.ElapsedMilliseconds);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "MCP Tool GetCurrentWeather failed for {City}: {Message}", city, ex.Message);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
        finally
        {
            stopwatch.Stop();
        }
    }

    /// <summary>
    /// Gets weather forecast for the specified city
    /// </summary>
    /// <param name="city">The city name to get weather forecast for</param>
    /// <param name="countryCode">Optional country code to narrow the search</param>
    /// <param name="days">Number of days for forecast (1-8)</param>
    /// <param name="units">Temperature units (metric, imperial, kelvin)</param>
    /// <param name="language">Language code for weather descriptions</param>
    /// <returns>Formatted weather forecast information</returns>
    [McpServerTool]
    [Description("Gets weather forecast for the specified city.")]
    public async Task<string> GetWeatherForecast(
        [Description("The city name to get weather forecast for")] string city,
        [Description("Optional: Country code (e.g., 'US', 'UK')")] string? countryCode = null,
        [Description("Number of days for forecast (1-8)")] int days = 5,
        [Description("Temperature units: 'metric' (Celsius), 'imperial' (Fahrenheit), 'kelvin'")] string units = "metric",
        [Description("Language code for weather descriptions (e.g., 'en', 'ru', 'es')")] string language = "en")
    {
        var command = new GetWeatherForecastCommand(city, countryCode, Math.Clamp(days, 1, 8), units, language);
        return await _mediator.Send(command);
    }

    /// <summary>
    /// Gets weather alerts and warnings for the specified city
    /// </summary>
    /// <param name="city">The city name to get weather alerts for</param>
    /// <param name="countryCode">Optional country code to narrow the search</param>
    /// <param name="language">Language code for alert descriptions</param>
    /// <returns>Formatted weather alerts information</returns>
    [McpServerTool]
    [Description("Gets weather alerts and warnings for the specified city.")]
    public async Task<string> GetWeatherAlerts(
        [Description("The city name to get weather alerts for")] string city,
        [Description("Optional: Country code (e.g., 'US', 'UK')")] string? countryCode = null,
        [Description("Language code for alert descriptions (e.g., 'en', 'ru', 'es')")] string language = "en")
    {
        var command = new GetWeatherAlertsCommand(city, countryCode, language);
        return await _mediator.Send(command);
    }
}