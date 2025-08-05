using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using WeatherMcpServer.Dtos;
using WeatherMcpServer.Models;

namespace WeatherMcpServer.Services;
public class WeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;
    private readonly OpenWeatherMapConfig _config;
    private readonly ILogger<WeatherService> _logger;
    public WeatherService(HttpClient httpClient, IOptions<OpenWeatherMapConfig> config, ILogger<WeatherService> logger)
    {
        _httpClient = httpClient;
        _config = config.Value;
        _logger = logger;
    }


    public async Task<LocationDto> GetLocation(string location)
    {           

        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("Location cannot be null or empty.");

        try
        {
            var url = $"/geo/1.0/direct?q={Uri.EscapeDataString(location)}&appid={_config.ApiKey}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get location for '{Location}'. StatusCode: {StatusCode}", location, response.StatusCode);
                throw new HttpRequestException($"Could not retrieve location '{location}'. Status: {response.StatusCode}");
            }

            var content = await response.Content.ReadAsStringAsync();
            var locations = JsonConvert.DeserializeObject<List<LocationDto>>(content);

            if (locations == null || !locations.Any())
            {
                _logger.LogInformation("No location data found for '{Location}'", location);
                throw new InvalidOperationException($"No matching location found for '{location}'.");
            }

            return locations.First();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting location for '{Location}'", location);
            throw;
        }
    }

    public async Task<WeatherDto> GetWeather(LocationDto location)
    {
        if (location == null)
            throw new ArgumentNullException(nameof(location));

        try
        { 
            var url = $"/data/3.0/onecall?lat={location.Latitude}&lon={location.Longitude}&appid={_config.ApiKey}&units=metric";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get weather for '{LocationName}'. StatusCode: {StatusCode}", location.Name, response.StatusCode);
                throw new HttpRequestException($"Could not retrieve weather for location '{location.Name}'. Status: {response.StatusCode}");
            }

            var content = await response.Content.ReadAsStringAsync();

            var weather = JsonConvert.DeserializeObject<WeatherDto>(content);

            if (weather == null)
            {
                _logger.LogWarning("Received empty weather data for '{LocationName}'", location.Name);
                throw new InvalidOperationException($"No weather data available for '{location.Name}'.");
            }

            return weather;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting weather for '{LocationName}'", location.Name);
            throw;
        }
    }
}
