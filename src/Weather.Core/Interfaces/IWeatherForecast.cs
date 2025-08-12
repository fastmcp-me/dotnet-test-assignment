using Weather.Core.Dto;

namespace Weather.Core.Interfaces;

public interface IWeatherForecast
{
	Task<WeatherDto> GetCurrentWeatherByCityAsync(string city, string? countryCode = null, string? stateCode = null, CancellationToken cancellationToken = default);
	Task<AlertDto> GetWeatherAlertAsync(double lat, double lon, DateTime date, CancellationToken cancellationToken = default);
	Task<WeatherDto> GetWeatherForecastByCityAsync(string city, DateTime date, string? countryCode = null, string? stateCode = null, CancellationToken cancellationToken = default);
}
