using WeatherMcpServer.Models;

namespace WeatherMcpServer.Services;

/// <summary>
/// Service for retrieving weather data from OpenWeatherMap API
/// </summary>
public interface IWeatherService
{
    /// <summary>
    /// Gets geographical coordinates for a city using OpenWeatherMap Geocoding API
    /// </summary>
    /// <param name="city">The city name to search for</param>
    /// <param name="countryCode">Optional country code to narrow the search (e.g., "US", "UK")</param>
    /// <param name="limit">Maximum number of locations to return (default: 1)</param>
    /// <returns>Array of matching locations with coordinates</returns>
    Task<GeolocationResponse[]> GetLocationAsync(string city, string? countryCode = null, int limit = 1);

    /// <summary>
    /// Gets comprehensive weather data for specified coordinates using OpenWeatherMap One Call API 3.0
    /// </summary>
    /// <param name="latitude">Latitude coordinate</param>
    /// <param name="longitude">Longitude coordinate</param>
    /// <param name="exclude">Comma-separated list of data parts to exclude (e.g., "hourly,daily")</param>
    /// <param name="units">Units of measurement: "metric", "imperial", or "kelvin"</param>
    /// <param name="language">Language code for weather descriptions (default: "en")</param>
    /// <returns>Complete weather data including current, hourly, daily, and alerts</returns>
    Task<WeatherResponse> GetWeatherAsync(double latitude, double longitude, string exclude = "", string units = "metric", string language = "en");
}