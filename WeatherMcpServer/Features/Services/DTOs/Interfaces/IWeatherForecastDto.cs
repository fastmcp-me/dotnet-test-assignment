namespace WeatherMcpServer.Features.Services.DTOs.Interfaces;

/// <summary>
/// Represents the weather forecast.
/// </summary>
public interface IWeatherForecastDto
{
    /// <summary>
    /// Gets or sets the city name.
    /// </summary>
    public string City { get; set; }

    /// <summary>
    /// Gets or sets the country code.
    /// </summary>
    public string Country { get; set; }

    /// <summary>
    /// Gets the daily forecast.
    /// </summary>
    public IDailyForecast[] Forecast { get; } 
}

/// <summary>
/// Represents the daily forecast.
/// </summary>
public interface IDailyForecast
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
    public string Description { get; set; }
}
