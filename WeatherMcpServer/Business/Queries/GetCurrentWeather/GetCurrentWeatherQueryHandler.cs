using MediatR;
using Microsoft.Extensions.Logging;
using WeatherMcpServer.Business.Interfaces;
using WeatherMcpServer.Business.Models;

namespace WeatherMcpServer.Business.Queries.GetCurrentWeather;

public class GetCurrentWeatherQueryHandler : IRequestHandler<GetCurrentWeatherQuery, CurrentWeatherData>
{
    private readonly IWeatherProvider _weatherProvider;
    private readonly ILogger<GetCurrentWeatherQueryHandler> _logger;

    public GetCurrentWeatherQueryHandler(
        IWeatherProvider weatherProvider,
        ILogger<GetCurrentWeatherQueryHandler> logger)
    {
        _weatherProvider = weatherProvider ?? throw new ArgumentNullException(nameof(weatherProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<CurrentWeatherData> Handle(GetCurrentWeatherQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing request to get current weather for {City}, {CountryCode}", 
            request.City, request.CountryCode);

        var result = await _weatherProvider.GetCurrentWeatherAsync(
            request.City, 
            request.CountryCode, 
            cancellationToken);

        _logger.LogInformation("Successfully retrieved current weather for {City}, {Country}", 
            result.City, result.Country);

        return result;
    }
}