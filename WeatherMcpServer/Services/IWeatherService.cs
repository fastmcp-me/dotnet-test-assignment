using WeatherMcpServer.Dtos;

namespace WeatherMcpServer.Services;

public interface IWeatherService
{
    /// <summary>
    /// Gets location information based on a location name.
    /// </summary>
    /// <param name="location">The location name to search for</param>
    /// <returns>Location information including coordinates</returns>
    /// <exception cref="ArgumentException">Thrown when location is null or empty</exception>
    /// <exception cref="InvalidOperationException">Thrown when no location is found</exception>
    Task<LocationDto> GetLocation(string location);

    /// <summary>
    /// Gets weather information for a specified location.
    /// </summary>
    /// <param name="location">The location for which to retrieve weather information</param>   
    /// <returns>Weather information for the specified location</returns>
    /// <exception cref="ArgumentNullException">Thrown when location is null</exception>
    /// <exception cref="HttpRequestException">Thrown when the weather service request fails</exception>
    /// <exception cref="InvalidOperationException">Thrown when no weather data is found</exception>
    Task<WeatherDto> GetWeather(LocationDto location);
}