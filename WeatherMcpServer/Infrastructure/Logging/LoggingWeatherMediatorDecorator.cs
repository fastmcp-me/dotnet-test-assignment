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
            _logger.LogInformation("Запрос текущей погоды для {Location}", location);
            try
            {
                var result = await _inner.GetCurrentWeatherAsync(location, ct);

                if (result.Success)
                    _logger.LogInformation("Успешно получили погоду: {Temp}°C", result.Value?.TemperatureC);
                else
                    _logger.LogWarning("Не удалось получить погоду: {Error}", result.Error);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении текущей погоды для {Location}", location);
                return Result<WeatherResult>.Fail("Ошибка при запросе погоды", ex);
            }
        }

        public async Task<Result<IEnumerable<WeatherResult>>> GetForecastAsync(string location, int days = 3, CancellationToken ct = default)
        {
            _logger.LogInformation("Запрос прогноза на {Days} дней для {Location}", days, location);
            try
            {
                var result = await _inner.GetForecastAsync(location, days, ct);

                if (result.Success)
                    _logger.LogInformation("Успешно получили прогноз ({Count} записей)", result.Value?.Count());
                else
                    _logger.LogWarning("Не удалось получить прогноз: {Error}", result.Error);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении прогноза для {Location}", location);
                return Result<IEnumerable<WeatherResult>>.Fail("Ошибка при запросе прогноза", ex);
            }
        }
    }
}
