using System.ComponentModel;
using MediatR;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using WeatherMcpServer.Business.Infrastructure.Exceptions;
using WeatherMcpServer.Business.Queries.GetCityWeather;

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
    [Description("Describes random weather in the provided city.")]
    public async Task<string> GetCityWeather(
        [Description("Name of the city to return weather for")] string city)
    {
        try
        {
            _logger.LogInformation("Getting weather for city: {City}", city);
            
            var query = new GetCityWeatherQuery(city);
            var response = await _mediator.Send(query);
            
            _logger.LogInformation("Successfully retrieved weather for city: {City}", city);
            
            return response.WeatherDescription;
        }
        catch (FluentValidation.ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed for city: {City}", city);
            var errors = string.Join("; ", ex.Errors.Select(e => e.ErrorMessage));
            throw new WeatherServiceException($"Invalid request: {errors}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting weather for city: {City}", city);
            throw new WeatherServiceException($"Failed to get weather for {city}. Please try again later.");
        }
    }
}