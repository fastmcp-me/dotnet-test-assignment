using MediatR;
using WeatherMcpServer.Models;

namespace WeatherMcpServer.Commands;

/// <summary>
/// Command to get current weather conditions for a specified city
/// </summary>
/// <param name="City">The city name to get weather for</param>
/// <param name="CountryCode">Optional country code to narrow the search</param>
/// <param name="Units">Temperature units: "metric", "imperial", or "kelvin"</param>
/// <param name="Language">Language code for weather descriptions</param>
public record GetCurrentWeatherCommand(
    string City,
    string? CountryCode = null,
    string Units = "metric",
    string Language = "en"
) : IRequest<string>;

/// <summary>
/// Command to get weather forecast for a specified city
/// </summary>
/// <param name="City">The city name to get weather forecast for</param>
/// <param name="CountryCode">Optional country code to narrow the search</param>
/// <param name="Days">Number of days for forecast (1-8)</param>
/// <param name="Units">Temperature units: "metric", "imperial", or "kelvin"</param>
/// <param name="Language">Language code for weather descriptions</param>
public record GetWeatherForecastCommand(
    string City,
    string? CountryCode = null,
    int Days = 5,
    string Units = "metric",
    string Language = "en"
) : IRequest<string>;

/// <summary>
/// Command to get weather alerts and warnings for a specified city
/// </summary>
/// <param name="City">The city name to get weather alerts for</param>
/// <param name="CountryCode">Optional country code to narrow the search</param>
/// <param name="Language">Language code for alert descriptions</param>
public record GetWeatherAlertsCommand(
    string City,
    string? CountryCode = null,
    string Language = "en"
) : IRequest<string>;