using MediatR;
using Microsoft.Extensions.Logging;
using WeatherMcpServer.Business.Interfaces;
using WeatherMcpServer.Business.Models;

namespace WeatherMcpServer.Business.Queries.GetWeatherAlerts;

public class GetWeatherAlertsQueryHandler : IRequestHandler<GetWeatherAlertsQuery, WeatherAlertsData>
{
    private readonly IWeatherProvider _weatherProvider;
    private readonly ILogger<GetWeatherAlertsQueryHandler> _logger;

    public GetWeatherAlertsQueryHandler(
        IWeatherProvider weatherProvider,
        ILogger<GetWeatherAlertsQueryHandler> logger)
    {
        _weatherProvider = weatherProvider ?? throw new ArgumentNullException(nameof(weatherProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<WeatherAlertsData> Handle(GetWeatherAlertsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing request to get weather alerts for {City}, {CountryCode}", 
            request.City, request.CountryCode);

        var result = await _weatherProvider.GetWeatherAlertsAsync(
            request.City, 
            request.CountryCode, 
            cancellationToken);

        _logger.LogInformation("Successfully retrieved weather alerts for {City}, {Country} with {Count} alerts", 
            result.City, result.Country, result.Alerts.Count);

        return result;
    }
}