using System.Text.Json;
using Microsoft.Extensions.Logging;
using WeatherMcpServer.Models;

namespace WeatherMcpServer.Services;

public interface IWeatherService
{
    Task<CurrentWeatherResponse?> GetCurrentWeatherAsync(string city, string? countryCode = null);
    Task<ForecastResponse?> GetWeatherForecastAsync(string city, string? countryCode = null, int days = 5);
    Task<WeatherAlertsResponse?> GetWeatherAlertsAsync(double lat, double lon);
}

public class WeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WeatherService> _logger;
    private readonly string _apiKey;
    private const string BaseUrl = "https://api.openweathermap.org/data/2.5";

    public WeatherService(HttpClient httpClient, ILogger<WeatherService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        
        _apiKey = Environment.GetEnvironmentVariable("OPENWEATHER_API_KEY") 
                  ?? throw new InvalidOperationException("OPENWEATHER_API_KEY environment variable is required");
    }

    public async Task<CurrentWeatherResponse?> GetCurrentWeatherAsync(string city, string? countryCode = null)
    {
        try
        {
            var location = string.IsNullOrEmpty(countryCode) ? city : $"{city},{countryCode}";
            var url = $"{BaseUrl}/weather?q={Uri.EscapeDataString(location)}&appid={_apiKey}&units=metric";
            
            _logger.LogInformation("Fetching current weather for {Location}", location);
            
            var response = await _httpClient.GetAsync(url);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Weather API error: {StatusCode} - {Content}", response.StatusCode, errorContent);
                return null;
            }

            var jsonContent = await response.Content.ReadAsStringAsync();
            var weatherData = JsonSerializer.Deserialize<CurrentWeatherResponse>(jsonContent);
            
            _logger.LogInformation("Successfully retrieved weather data for {Location}", location);
            return weatherData;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching current weather for {City}", city);
            return null;
        }
    }

    public async Task<ForecastResponse?> GetWeatherForecastAsync(string city, string? countryCode = null, int days = 5)
    {
        try
        {
            var location = string.IsNullOrEmpty(countryCode) ? city : $"{city},{countryCode}";
            // OpenWeatherMap free tier provides 5-day forecast with 3-hour intervals
            var cnt = Math.Min(days * 8, 40); // 8 intervals per day, max 40 (5 days)
            var url = $"{BaseUrl}/forecast?q={Uri.EscapeDataString(location)}&appid={_apiKey}&units=metric&cnt={cnt}";
            
            _logger.LogInformation("Fetching {Days}-day weather forecast for {Location}", days, location);
            
            var response = await _httpClient.GetAsync(url);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Weather forecast API error: {StatusCode} - {Content}", response.StatusCode, errorContent);
                return null;
            }

            var jsonContent = await response.Content.ReadAsStringAsync();
            var forecastData = JsonSerializer.Deserialize<ForecastResponse>(jsonContent);
            
            _logger.LogInformation("Successfully retrieved forecast data for {Location}", location);
            return forecastData;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching weather forecast for {City}", city);
            return null;
        }
    }

    public Task<WeatherAlertsResponse?> GetWeatherAlertsAsync(double lat, double lon)
    {
        try
        {
            // Note: Weather alerts require One Call API which needs a paid subscription
            // For this implementation, we'll use a mock response or return null
            // In a real implementation, you would use: /onecall?lat={lat}&lon={lon}&appid={apiKey}&exclude=current,minutely,hourly,daily
            
            _logger.LogInformation("Weather alerts feature requires One Call API subscription");
            _logger.LogInformation("Requested alerts for coordinates: {Lat}, {Lon}", lat, lon);
            
            // Return empty alerts for now
            return Task.FromResult<WeatherAlertsResponse?>(new WeatherAlertsResponse { Alerts = Array.Empty<WeatherAlert>() });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching weather alerts for coordinates {Lat}, {Lon}", lat, lon);
            return Task.FromResult<WeatherAlertsResponse?>(null);
        }
    }
}
