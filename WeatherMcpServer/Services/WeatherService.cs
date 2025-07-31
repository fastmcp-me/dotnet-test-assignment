using System.Text.Json;
using Serilog;
using WeatherMcpServer.Models;
using System.Diagnostics;

namespace WeatherMcpServer.Services;

/// <summary>
/// Implementation of weather service using OpenWeatherMap API
/// </summary>
public class WeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger _logger;
    private readonly string _apiKey;

    /// <summary>
    /// Initializes a new instance of the WeatherService
    /// </summary>
    /// <param name="httpClient">HTTP client configured for OpenWeatherMap API</param>
    /// <param name="logger">Serilog logger instance for structured logging</param>
    /// <exception cref="InvalidOperationException">Thrown when OPENWEATHERMAP_API_KEY environment variable is not set</exception>
    public WeatherService(HttpClient httpClient, ILogger logger)
    {
        _httpClient = httpClient;
        _logger = logger.ForContext<WeatherService>();
        _apiKey = Environment.GetEnvironmentVariable("OPENWEATHERMAP_API_KEY") 
                  ?? throw new InvalidOperationException("OPENWEATHERMAP_API_KEY environment variable is required");
        
        _logger.Debug("WeatherService initialized with base address: {BaseAddress}", _httpClient.BaseAddress);
    }

    /// <inheritdoc />
    public async Task<GeolocationResponse[]> GetLocationAsync(string city, string? countryCode = null, int limit = 1)
    {
        using var activity = new Activity("GetLocation").Start();
        activity?.SetTag("weather.city", city);
        activity?.SetTag("weather.country_code", countryCode);
        activity?.SetTag("weather.limit", limit);

        var query = string.IsNullOrEmpty(countryCode) ? city : $"{city},{countryCode}";
        var url = $"geo/1.0/direct?q={Uri.EscapeDataString(query)}&limit={limit}&appid={_apiKey}";
        
        _logger.Information("Getting location for city: {City}, country: {CountryCode}, limit: {Limit}", 
            city, countryCode, limit);
        _logger.Debug("Geocoding API URL: {Url}", url.Replace(_apiKey, "***"));

        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            var response = await _httpClient.GetAsync(url);
            
            _logger.Debug("Geocoding API response: {StatusCode} in {ElapsedMs}ms", 
                response.StatusCode, stopwatch.ElapsedMilliseconds);
            
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync();
            _logger.Verbose("Geocoding API response body: {ResponseBody}", json);
            
            var locations = JsonSerializer.Deserialize<GeolocationResponse[]>(json) ?? [];
            
            _logger.Information("Found {Count} locations for {City} in {ElapsedMs}ms", 
                locations.Length, city, stopwatch.ElapsedMilliseconds);
            
            if (locations.Length > 0)
            {
                _logger.Debug("Top location: {LocationName}, {Country} ({Latitude}, {Longitude})", 
                    locations[0].Name, locations[0].Country, locations[0].Latitude, locations[0].Longitude);
            }
            
            activity?.SetTag("weather.locations_found", locations.Length);
            return locations;
        }
        catch (HttpRequestException ex)
        {
            _logger.Error(ex, "HTTP error while getting location for {City}: {Message}", city, ex.Message);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
        catch (JsonException ex)
        {
            _logger.Error(ex, "JSON deserialization error while getting location for {City}: {Message}", city, ex.Message);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Unexpected error while getting location for {City}: {Message}", city, ex.Message);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
        finally
        {
            stopwatch.Stop();
            activity?.SetTag("weather.duration_ms", stopwatch.ElapsedMilliseconds);
        }
    }

    /// <inheritdoc />
    public async Task<WeatherResponse> GetWeatherAsync(double latitude, double longitude, string exclude = "", string units = "metric", string language = "en")
    {
        using var activity = new Activity("GetWeather").Start();
        activity?.SetTag("weather.latitude", latitude);
        activity?.SetTag("weather.longitude", longitude);
        activity?.SetTag("weather.units", units);
        activity?.SetTag("weather.language", language);
        activity?.SetTag("weather.exclude", exclude);

        var url = $"data/3.0/onecall?lat={latitude}&lon={longitude}&units={units}&lang={language}&appid={_apiKey}";
        
        if (!string.IsNullOrEmpty(exclude))
        {
            url += $"&exclude={exclude}";
        }
        
        _logger.Information("Getting weather for coordinates: {Latitude}, {Longitude} with units: {Units}, language: {Language}", 
            latitude, longitude, units, language);
        _logger.Debug("Weather API URL: {Url}, exclude: {Exclude}", url.Replace(_apiKey, "***"), exclude);

        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            var response = await _httpClient.GetAsync(url);
            
            _logger.Debug("Weather API response: {StatusCode} in {ElapsedMs}ms", 
                response.StatusCode, stopwatch.ElapsedMilliseconds);
            
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync();
            _logger.Verbose("Weather API response body length: {JsonLength} characters", json.Length);
            
            var weather = JsonSerializer.Deserialize<WeatherResponse>(json) 
                          ?? throw new InvalidOperationException("Failed to deserialize weather response");
            
            _logger.Information("Successfully retrieved weather data for ({Latitude}, {Longitude}) in {ElapsedMs}ms. " +
                                 "Current temp: {CurrentTemp}, timezone: {Timezone}", 
                latitude, longitude, stopwatch.ElapsedMilliseconds, 
                weather.Current?.Temperature, weather.Timezone);
            
            // Log additional context about what data was retrieved
            var dataTypes = new List<string>();
            if (weather.Current != null) dataTypes.Add("current");
            if (weather.Minutely?.Length > 0) dataTypes.Add($"minutely({weather.Minutely.Length})");
            if (weather.Hourly?.Length > 0) dataTypes.Add($"hourly({weather.Hourly.Length})");
            if (weather.Daily?.Length > 0) dataTypes.Add($"daily({weather.Daily.Length})");
            if (weather.Alerts?.Length > 0) dataTypes.Add($"alerts({weather.Alerts.Length})");
            
            _logger.Debug("Retrieved weather data types: {DataTypes}", string.Join(", ", dataTypes));
            
            activity?.SetTag("weather.data_types", string.Join(",", dataTypes));
            activity?.SetTag("weather.timezone", weather.Timezone);
            
            return weather;
        }
        catch (HttpRequestException ex)
        {
            _logger.Error(ex, "HTTP error while getting weather for ({Latitude}, {Longitude}): {Message}", 
                latitude, longitude, ex.Message);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
        catch (JsonException ex)
        {
            _logger.Error(ex, "JSON deserialization error while getting weather for ({Latitude}, {Longitude}): {Message}", 
                latitude, longitude, ex.Message);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Unexpected error while getting weather for ({Latitude}, {Longitude}): {Message}", 
                latitude, longitude, ex.Message);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
        finally
        {
            stopwatch.Stop();
            activity?.SetTag("weather.duration_ms", stopwatch.ElapsedMilliseconds);
        }
    }
}