using MediatR;
using Microsoft.Extensions.Logging;
using WeatherMcpServer.Business.Interfaces;
using WeatherMcpServer.Business.Models;

namespace WeatherMcpServer.Business.Queries.GetWeatherForecast;

public class GetWeatherForecastQueryHandler : IRequestHandler<GetWeatherForecastQuery, WeatherForecastData>
{
    private readonly IWeatherProvider _weatherProvider;
    private readonly ILogger<GetWeatherForecastQueryHandler> _logger;

    public GetWeatherForecastQueryHandler(
        IWeatherProvider weatherProvider,
        ILogger<GetWeatherForecastQueryHandler> logger)
    {
        _weatherProvider = weatherProvider ?? throw new ArgumentNullException(nameof(weatherProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<WeatherForecastData> Handle(GetWeatherForecastQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing request to get weather forecast for {City}, {CountryCode} for {Days} days", 
            request.City, request.CountryCode, request.Days);

        var result = await _weatherProvider.GetWeatherForecastAsync(
            request.City, 
            request.CountryCode, 
            request.Days,
            cancellationToken);

        _logger.LogInformation("Successfully retrieved weather forecast for {City}, {Country} with {Count} daily forecasts", 
            result.City, result.Country, result.DailyForecasts.Count);

        return result;
    }
}