using Domain.Common.Weathers;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace Api.Tools;

public class WeatherTools
{
    #region DI
    private readonly IWeatherClient _client;

    public WeatherTools(IWeatherClient client)
    {
        _client = client;
    }
    #endregion

    /// <summary>
    /// Get current weather for a specified location
    /// </summary>
    /// <param name="city"></param>
    /// <param name="country"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [McpServerTool]
    [Description("Gets current weather conditions for the specified city.")]
    public async Task<string> GetCurrentWeather(
        [Description("City name, e.g. 'Almaty'")] string city,
        [Description("Optional ISO country code, e.g. 'KZ'")] string? countryCode = null,
        CancellationToken ct = default)
    {
        var w = await _client.GetCurrentAsync(city, countryCode, ct);
        return $"🌡️ {w.TempC:0.#}°C, {w.Description} (as of {w.TimeUtc:HH:mm} UTC)";
    }


    /// <summary>
    /// Get weather forecast for a specified location
    /// </summary>
    /// <param name="city"></param>
    /// <param name="country"></param>
    /// <param name="days"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [McpServerTool]
    [Description("Returns 3-day forecast for the city.")]
    public async Task<IEnumerable<string>> GetWeatherForecast(
        [Description("City name")] string city,
        [Description("Optional ISO country code")] string? countryCode = null,
        [Description("Number of days (1–5)")] int days = 3,
        CancellationToken ct = default)
    {
        var forecast = await _client.GetForecastAsync(city, countryCode, days, ct);

        return forecast.Select(f =>
            $"{f.Date:dddd}: 🌡️ {f.TempC:0.#}°C, {f.Description}");
    }


    /// <summary>
    /// Get weather alerts/warnings for a location (bonus)
    /// </summary>
    /// <param name="city"></param>
    /// <param name="country"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [McpServerTool]
    [Description("Returns current weather alerts for the city (BONUS).")]
    public async Task<string?> GetWeatherAlerts(
        [Description("City name")] string city,
        [Description("Optional ISO country code")] string? countryCode = null,
        CancellationToken ct = default)
    {
        return await _client.GetAlertsAsync(city, countryCode, ct);
    }


    [McpServerTool]
    [Description("Describes random weather in the provided city.")]
    public string GetCityWeather(
        [Description("Name of the city to return weather for")] string city)
    {
        // Read the environment variable during tool execution.
        // Alternatively, this could be read during startup and passed via IOptions dependency injection
        var weather = Environment.GetEnvironmentVariable("WEATHER_CHOICES");
        if (string.IsNullOrWhiteSpace(weather))
        {
            weather = "balmy,rainy,stormy";
        }

        var weatherChoices = weather.Split(",");
        var selectedWeatherIndex = Random.Shared.Next(0, weatherChoices.Length);

        return $"The weather in {city} is {weatherChoices[selectedWeatherIndex]}.";
    }

}