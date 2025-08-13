using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Globalization;
using WeatherMcpServer.Domain.LocationAggregate;
using WeatherMcpServer.Infrastructure.Exceptions;

namespace WeatherMcpServer.Infrastructure.OpenWeather;

public sealed class OpenWeatherProvider :
    HttpProvider
    , IWeatherProvider
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<OpenWeatherProvider> _logger;
    private readonly string _apiKey;


    public OpenWeatherProvider(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<OpenWeatherProvider> logger)
        : base(httpClient, logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
        _apiKey = configuration["OpenWeather:ApiKey"]
                  ?? throw new ArgumentNullException(nameof(configuration), "OpenWeather API key is not configured.");
    }


    public async Task<(double lat, double lon)?> GetCoordinatesAsync(Location location)
    {
        var query = location.CountryCode != null ? $"{location.City},{location.CountryCode}" : location.City;
        var url = $"geo/1.0/direct?q={query}&limit=5&appid={_apiKey}";

        var result = await GetAsync<List<GeocodingResponseDto>>(url);

        if (result is null || result.Count == 0)
        {
            _logger.LogWarning("No coordinates found for city '{City}'.", location.City);
            return null;
        }

        var loc = result.First();
        return (loc.Lat, loc.Lon);
    }

    public async Task<WeatherInfo> GetCurrentWeatherAsync(Location location)
    {
        var query = location.CountryCode != null ? $"{location.City},{location.CountryCode}" : location.City;
        var url = $"data/2.5/weather?q={query}&units=metric&appid={_apiKey}";

        var result = await GetAsync<CurrentWeatherResponseDto>(url);

        if (result == null)
        {
            _logger.LogWarning("No weather data returned for city '{City}'.", location.City);
            throw new ExternalServiceException($"No weather data for {location}");
        }

        var info = WeatherInfo.Create(
            result.Weather.FirstOrDefault()?.Description ?? "No description",
            result.Main.Temp,
            result.Main.Feels_Like,
            humidity: result.Main.Humidity);

        return info;
    }

    /// <summary>
    /// Weather alerts are not available for my subscription :((
    /// </summary>
    public Task<IEnumerable<WeatherAlert>> GetWeatherAlertsAsync(Location location)
    {
        _logger.LogInformation(
            "Weather alerts are not available on the free OpenWeather API plan. Returning empty list for {City}, {CountryCode}",
            location.City,
            location.CountryCode ?? "N/A");

        return Task.FromResult<IEnumerable<WeatherAlert>>([]);
    }

    public async Task<IEnumerable<WeatherForecast>> GetWeatherForecastAsync(Location location, int days)
    {
        var coords = await GetCoordinatesAsync(location);

        if (coords == null)
        {
            _logger.LogWarning("Cannot get forecast: coordinates not found for {City}", location.City);
            return [];
        }

        var (lat, lon) = coords.Value;

        var url = $"data/2.5/forecast?lat={lat.ToString(CultureInfo.InvariantCulture)}" +
                  $"&lon={lon.ToString(CultureInfo.InvariantCulture)}" +
                  $"&units=metric&appid={_apiKey}";

        var result = await GetAsync<ForecastResponseDto>(url);

        if (result?.List == null || result.List.Count == 0)
        {
            _logger.LogWarning("No forecast data for {City}", location.City);
            return [];
        }

        var groupedByDay = result.List
            .GroupBy(x => DateTimeOffset
                .FromUnixTimeSeconds(x.Dt)
                .Date)
            .OrderBy(g => g.Key)
            .Take(days);

        var forecasts = groupedByDay
            .Select(g =>
            {
                var avgTemp = g.Average(x => x.Main.Temp);
                var description = g
                    .SelectMany(x => x.Weather)
                    .GroupBy(w => w.Description)
                    .OrderByDescending(grp => grp.Count())
                    .First().Key;

                return WeatherForecast.Create(
                    date: g.Key,
                    description: description,
                    temperatureCelsius: (decimal)avgTemp
                );
            })
            .ToList();

        return forecasts;
    }


}
