using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WeatherMcpServer.Business.Infrastructure.Exceptions;
using WeatherMcpServer.Business.Interfaces;
using WeatherMcpServer.Business.Models;
using WeatherMcpServer.Integrations.OpenWeatherMap.Models;

namespace WeatherMcpServer.Integrations.OpenWeatherMap.Services;

public class OpenWeatherMapService : IWeatherProvider
{
    private readonly HttpClient _httpClient;
    private readonly OpenWeatherMapConfiguration _configuration;
    private readonly ILogger<OpenWeatherMapService> _logger;

    public OpenWeatherMapService(
        HttpClient httpClient,
        IOptions<OpenWeatherMapConfiguration> configuration,
        ILogger<OpenWeatherMapService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _configuration = configuration?.Value ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        ValidateConfiguration();
    }

    public async Task<CurrentWeatherData> GetCurrentWeatherAsync(string city, string? countryCode = null, CancellationToken cancellationToken = default)
    {
        var location = BuildLocationQuery(city, countryCode);
        var url = $"weather?q={location}&appid={_configuration.ApiKey}&units={_configuration.Units}";
        
        _logger.LogInformation("Fetching current weather for {Location}", location);
        
        var response = await _httpClient.GetFromJsonAsync<OpenWeatherMapCurrentResponse>(url, cancellationToken);
        
        if (response == null)
        {
            throw new WeatherServiceException($"No weather data received for {location}");
        }
        
        return MapToCurrentWeatherData(response);
    }

    public async Task<WeatherForecastData> GetWeatherForecastAsync(string city, string? countryCode = null, int days = 3, CancellationToken cancellationToken = default)
    {
        var location = BuildLocationQuery(city, countryCode);
        var url = $"forecast?q={location}&appid={_configuration.ApiKey}&units={_configuration.Units}&cnt={days * 8}"; // 8 forecasts per day (3-hour intervals)
        
        _logger.LogInformation("Fetching weather forecast for {Location} for {Days} days", location, days);
        
        var response = await _httpClient.GetFromJsonAsync<OpenWeatherMapForecastResponse>(url, cancellationToken);
        
        if (response == null)
        {
            throw new WeatherServiceException($"No forecast data received for {location}");
        }
        
        return MapToWeatherForecastData(response, days);
    }

    public async Task<WeatherAlertsData> GetWeatherAlertsAsync(string city, string? countryCode = null, CancellationToken cancellationToken = default)
    {
        // OpenWeatherMap requires a paid subscription for alerts
        // For this implementation, we'll return an empty alerts list
        _logger.LogInformation("Weather alerts requested for {City}, but this feature requires a paid OpenWeatherMap subscription", city);
        
        return await Task.FromResult(new WeatherAlertsData
        {
            City = city,
            Country = countryCode ?? string.Empty,
            Alerts = new List<WeatherAlert>()
        });
    }

    private void ValidateConfiguration()
    {
        if (string.IsNullOrWhiteSpace(_configuration.ApiKey))
        {
            throw new WeatherServiceException("OpenWeatherMap API key is not configured. Please set the API key in configuration or environment variables.");
        }
        
        _httpClient.BaseAddress = new Uri(_configuration.BaseUrl);
        _httpClient.Timeout = TimeSpan.FromSeconds(_configuration.TimeoutSeconds);
    }

    private static string BuildLocationQuery(string city, string? countryCode)
    {
        return string.IsNullOrWhiteSpace(countryCode) 
            ? city 
            : $"{city},{countryCode}";
    }

    private static CurrentWeatherData MapToCurrentWeatherData(OpenWeatherMapCurrentResponse response)
    {
        return new CurrentWeatherData
        {
            City = response.Name,
            Country = response.Sys.Country,
            Temperature = Math.Round(response.Main.Temp, 1),
            FeelsLike = Math.Round(response.Main.FeelsLike, 1),
            Description = response.Weather.FirstOrDefault()?.Description ?? "Unknown",
            MainCondition = response.Weather.FirstOrDefault()?.Main ?? "Unknown",
            Humidity = response.Main.Humidity,
            WindSpeed = Math.Round(response.Wind.Speed, 1),
            Pressure = response.Main.Pressure,
            Timestamp = DateTimeOffset.FromUnixTimeSeconds(response.Dt).DateTime,
            Icon = response.Weather.FirstOrDefault()?.Icon ?? string.Empty
        };
    }

    private static WeatherForecastData MapToWeatherForecastData(OpenWeatherMapForecastResponse response, int days)
    {
        var dailyForecasts = response.List
            .GroupBy(item => DateTimeOffset.FromUnixTimeSeconds(item.Dt).Date)
            .Take(days)
            .Select(group => new DailyForecast
            {
                Date = group.Key,
                TemperatureMin = Math.Round(group.Min(item => item.Main.TempMin), 1),
                TemperatureMax = Math.Round(group.Max(item => item.Main.TempMax), 1),
                Temperature = Math.Round(group.Average(item => item.Main.Temp), 1),
                Description = group.First().Weather.FirstOrDefault()?.Description ?? "Unknown",
                MainCondition = group.First().Weather.FirstOrDefault()?.Main ?? "Unknown",
                Humidity = (int)group.Average(item => item.Main.Humidity),
                WindSpeed = Math.Round(group.Average(item => item.Wind.Speed), 1),
                PrecipitationProbability = Math.Round(group.Max(item => item.Pop) * 100, 0),
                Icon = group.First().Weather.FirstOrDefault()?.Icon ?? string.Empty
            })
            .ToList();

        return new WeatherForecastData
        {
            City = response.City.Name,
            Country = response.City.Country,
            DailyForecasts = dailyForecasts
        };
    }
}