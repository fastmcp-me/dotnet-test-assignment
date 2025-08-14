using Microsoft.Extensions.Logging;
using WeatherMcpServer.Application.Abstractions;
using WeatherMcpServer.Domain.LocationAggregate;

namespace WeatherMcpServer.Application;

internal sealed class WeatherService : IWeatherService
{
    private readonly IWeatherProvider _weatherProvider;
    private readonly ICacheService _cache;
    private readonly ILogger _logger;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(30);

    public WeatherService(
        IWeatherProvider weatherProvider,
        ICacheService cache,
        ILogger logger)
    {
        _weatherProvider = weatherProvider;
        _cache = cache;
        _logger = logger;
    }

    public async Task<LocationWeather> GetLocationWeatherAsync(Location location, int forecastDays = 3)
    {
        string cacheKey = $"{location.City}_{location.CountryCode}_{forecastDays}";

        var cachedWeather = _cache.Get<LocationWeather>(cacheKey);

        if (cachedWeather is not null)
        {
            _logger.LogInformation("Returning cached weather data for {Location}", cacheKey);
            return cachedWeather!;
        }

        var currentWeatherTask = _weatherProvider.GetCurrentWeatherAsync(location);
        var forecastsTask = _weatherProvider.GetWeatherForecastAsync(location, forecastDays);
        var alertsTask = _weatherProvider.GetWeatherAlertsAsync(location);

        try
        {
            await Task.WhenAll(currentWeatherTask, forecastsTask, alertsTask);

            var locationWeather =  LocationWeather.Create(
                location, 
                currentWeatherTask.Result, 
                forecastsTask.Result, 
                alertsTask.Result);

            _cache.Set(cacheKey, locationWeather, _cacheDuration);

            _logger.LogInformation("Weather data fetched and cached for {Location}", cacheKey);

            return locationWeather;
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "Invalid argument provided for location {Location}", cacheKey);
            throw new WeatherServiceException("Invalid input data", ex);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Operation error while fetching weather for {Location}", cacheKey);
            throw new WeatherServiceException("Error fetching weather data", ex);
        }
    }
}