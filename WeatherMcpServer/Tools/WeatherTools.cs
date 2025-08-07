using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModelContextProtocol.Server;
using WeatherMcpServer.Configurations;
using WeatherMcpServer.Features.Queries;
using System.ComponentModel;

namespace WeatherMcpServer.Tools;

public class WeatherTools
{
    private readonly ILogger<WeatherTools> _logger;
    private readonly WeatherApiConfiguration _options;
    private readonly IMediator _mediator;

    public WeatherTools(ILogger<WeatherTools> logger, IOptions<WeatherApiConfiguration> options, IMediator mediator)
    {
        _logger = logger;
        _options = options.Value;
        _logger.LogInformation("WeatherTools initialized with API Key: {ApiKey}", _options.ApiKey?.Replace(_options.ApiKey, "***"));
        _mediator = mediator;
    }

    [McpServerTool]
    [Description("Gets current weather conditions for the specified city.")]
    public Task<string> GetCurrentWeather(
        [Description("The city name to get weather for")] string city,
        [Description("Optional: Country code (e.g., 'US', 'UK')")] string? countryCode = null,
        CancellationToken cancellationToken = default)
    {
        return _mediator.Send(new GetCurrentWeather.Query(city, countryCode), cancellationToken);
    }

    [McpServerTool]
    [Description("Get weather forecast for a specified location.")]
    public Task<string> GetWeatherForecast(
        [Description("The city name to get weather for")] string city,
        [Description("Optional: Country code (e.g., 'US', 'UK')")] string? countryCode = null,
        [Description("Optional: Number of days to forecast (min: 1, max: 5). Default is 3.")] int days = 3,
        CancellationToken cancellationToken = default)
    {
        return _mediator.Send(new GetWeatherForecast.Query(city, countryCode, days), cancellationToken);
    }

    [McpServerTool]
    [Description("Get weather alerts/warnings for a location.")]
    public Task<string> GetWeatherAlerts(
        [Description("The city name to get weather alerts for")] string city,
        [Description("Optional: Country code (e.g., 'US', 'UK')")] string? countryCode = null,
        CancellationToken cancellationToken = default)
    {
        return _mediator.Send(new GetWeatherAlerts.Query(city, countryCode), cancellationToken);
    }
}