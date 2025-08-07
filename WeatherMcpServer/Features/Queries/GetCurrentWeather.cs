using MediatR;
using WeatherMcpServer.Features.Services.Interfaces;

namespace WeatherMcpServer.Features.Queries;

/// <summary>
/// Contains the query and handler for getting the current weather.
/// </summary>
public static class GetCurrentWeather
{
    /// <summary>
    /// Represents the query for getting the current weather.
    /// </summary>
    /// <param name="City">The city name.</param>
    /// <param name="CountryCode">The country code.</param>
    public record Query(string City, string? CountryCode)
        : WeatherBase.Query(City, CountryCode);

    /// <summary>
    /// Handles the <see cref="Query"/> for getting the current weather.
    /// </summary>
    internal class Handler(IWeatherService weatherService) : IRequestHandler<Query, string>
    {
        public async Task<string> Handle(Query request, CancellationToken cancellationToken)
        {
            var weatherData = await weatherService.GetCurrentWeather(request.City, request.CountryCode, cancellationToken);
            return $"""
Current weather in {weatherData.City}, {weatherData.Country}:
🌡️ Temperature: {weatherData.Temperature:F1}° (feels like {weatherData.TemperatureFeelsLike:F1}°)
☁️ Conditions: {weatherData.Description}
💨 Wind: {weatherData.WindSpeed:F1} m/s
💧 Humidity: {weatherData.Humidity}%
🔽 Pressure: {weatherData.Pressure} hPa
""";
        }
    }

    /// <summary>
    /// Validates the <see cref="Query"/>.
    /// </summary>
    internal class Validator : WeatherBase.Validator<Query>
    {
        public Validator()
        {
        }
    }
}
