using WeatherMcpServer.Features.Services.DTOs.Interfaces;

namespace WeatherMcpServer.Features.Services.DTOs.OpenWeatherMap.WeatherForecast;

/// <summary>
/// Represents the data transfer object for the weather forecast.
/// </summary>
internal class WeatherForecastDto : IWeatherForecastDto
{
    /// <summary>
    /// Gets or sets the city name.
    /// </summary>
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the country code.
    /// </summary>
    public string Country { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the daily forecast.
    /// </summary>
    public DailyForecast[] Forecast { get; set; } = [];
    IDailyForecast[] IWeatherForecastDto.Forecast => Forecast;
}

/// <summary>
/// Represents the data transfer object for the daily forecast.
/// </summary>
internal class DailyForecast : IDailyForecast
{
    /// <summary>
    /// Gets or sets the date of the forecast.
    /// </summary>
    public DateOnly Date { get; set; }

    /// <summary>
    /// Gets or sets the minimum temperature.
    /// </summary>
    public double MinTemperature { get; set; }

    /// <summary>
    /// Gets or sets the maximum temperature.
    /// </summary>
    public double MaxTemperature { get; set; }

    /// <summary>
    /// Gets or sets the description of the weather.
    /// </summary>
    public string Description { get; set; } = string.Empty;
}
