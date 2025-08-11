using Microsoft.Extensions.Logging;
using WeatherMcpServer.Application.Interfaces;
using WeatherMcpServer.Domain;

namespace WeatherMcpServer.Infrastructure.Logging
{
    public class LoggingWeatherMediatorDecorator : IWeatherMediator
    {
        private readonly IWeatherMediator _inner;
        private readonly ILogger<LoggingWeatherMediatorDecorator> _logger;

        public LoggingWeatherMediatorDecorator(IWeatherMediator inner, ILogger<LoggingWeatherMediatorDecorator> logger)
        {
            _inner = inner;
            _logger = logger;
        }

        public async Task<Result<WeatherResult>> GetCurrentWeatherAsync(string location, CancellationToken ct = default)
        {
            _logger.LogInformation("Request current weather for {Location}", location);
            try
            {
                var result = await _inner.GetCurrentWeatherAsync(location, ct);

                if (result.Success)
                    _logger.LogInformation("Successfully received the weather: {Temp}°C", result.Value?.TemperatureC);
                else
                    _logger.LogWarning("Failed to get weather: {Error}", result.Error);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current weather for {Location}", location);
                return Result<WeatherResult>.Fail("Error while requesting weather", ex);
            }
        }

        public async Task<Result<IEnumerable<WeatherResult>>> GetForecastAsync(string location, int days = 3, CancellationToken ct = default)
        {
            _logger.LogInformation("Request forecast for {Days} days for {Location}", days, location);
            try
            {
                var result = await _inner.GetForecastAsync(location, days, ct);

                if (result.Success)
                    _logger.LogInformation("Successfully received forecast ({Count} records)", result.Value?.Count());
                else
                    _logger.LogWarning("Failed to get forecast: {Error}", result.Error);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting forecast for {Location}", location);
                return Result<IEnumerable<WeatherResult>>.Fail("Error requesting forecast", ex);
            }
        }
    }
}
