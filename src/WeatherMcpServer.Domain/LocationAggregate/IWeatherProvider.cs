namespace WeatherMcpServer.Domain.LocationAggregate;

public interface IWeatherProvider
{
    Task<WeatherInfo> GetCurrentWeatherAsync(Location location);
    Task<IEnumerable<WeatherForecast>> GetWeatherForecastAsync(Location location, int days);
    Task<IEnumerable<WeatherAlert>> GetWeatherAlertsAsync(Location location);
}
