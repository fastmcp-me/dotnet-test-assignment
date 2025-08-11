using Microsoft.Extensions.Logging;
using WeatherMcpServer.Application.Interfaces;
using WeatherMcpServer.Domain;

namespace WeatherMcpServer.Infrastructure.Providers;

public class LoggingWeatherProviderDecorator : IWeatherProvider
{
    private readonly IWeatherProvider _inner;
    private readonly ILogger<LoggingWeatherProviderDecorator> _logger;

    public string Name => _inner.Name;
    public int Weight => _inner.Weight;

    public LoggingWeatherProviderDecorator(
        IWeatherProvider inner,
        ILogger<LoggingWeatherProviderDecorator> logger)
    {
        _inner = inner;
        _logger = logger;
    }

    public async Task<WeatherResult> GetCurrentWeatherAsync(string location, CancellationToken ct = default)
    {
        _logger.LogInformation("[{Provider}] Get current weather for {Location}", Name, location);

        try
        {
            var result = await _inner.GetCurrentWeatherAsync(location, ct);
            _logger.LogInformation("[{Provider}] Weather in {Location}: {Temp}°C, {Desc}",
                Name, location, result.TemperatureC, result.Description);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[{Provider}] Error getting current weather for {Location}", Name, location);
            throw;
        }
    }

    public async Task<IEnumerable<WeatherResult>> GetForecastAsync(string location, int days = 3, CancellationToken ct = default)
    {
        _logger.LogInformation("[{Provider}] Get forecast for {Days} days for {Location}", Name, days, location);

        try
        {
            var result = await _inner.GetForecastAsync(location, days, ct);
            _logger.LogInformation("[{Provider}] Forecast received ({Count} records)", Name, result.Count());
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[{Provider}] Error getting forecast for {Location}", Name, location);
            throw;
        }
    }
}
