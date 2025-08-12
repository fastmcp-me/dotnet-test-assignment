using Weather.Core.Interfaces;
using ModelContextProtocol.Server;
using System.ComponentModel;
using Weather.UseCases.Forecast.GetCityWeather;
using Weather.UseCases.Forecast.GetCityForecast;
using Weather.UseCases.Forecast.GetAlert;

namespace WeatherMcpServer.Tools;

public class WeatherTools(IUseCaseDispatcher dispatcher)
{
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
        var selectedWeatherIndex =  Random.Shared.Next(0, weatherChoices.Length);

        return $"The weather in {city} is {weatherChoices[selectedWeatherIndex]}.";
    }

	[McpServerTool]
	[Description("Returns current weather in the provided city with optional country code and state code")]
	public async Task<string> GetCurrentWeather(
		[Description("Name of the city to return weather for")] string city,
		[Description("Optional ISO 3166 country code of the city")] string? countryCode = null,
		[Description("Optional state code of the city in USA.  Use the 2 letter abbreviation")] string? stateCode = null,
		CancellationToken cancellationToken = default
		)
    {
		return await dispatcher.Send(new GetCityWeatherRequest(city, countryCode, stateCode), cancellationToken);
    }

	[McpServerTool]
	[Description("Returns weather forecast in the provided city with optional country code and state code")]
	public async Task<string> GetWeatherForecast(
		[Description("Name of the city to return weather for")] string city,
		[Description("The date for which to return the forecast (in ISO 8601 format, UTC preferred, e.g., 2025-08-11T00:00:00Z).")] DateTime date,
		[Description("Optional ISO 3166 country code of the city")] string? countryCode = null,
		[Description("Optional state code of the city in USA. Use the 2 letter abbreviation")] string? stateCode = null,
		CancellationToken cancellationToken = default
		)
    {
		return await dispatcher.Send(new GetCityForecastRequest(city, date, countryCode, stateCode), cancellationToken);
	}

	[McpServerTool]
	[Description("Returns weather alerts for a location. ")]
	public async Task<string> GetWeatherAlerts(
		[Description("Latitude of the location.")] double latitude,
		[Description("Longitude of the location.")] double longitude,
		[Description("The date for which to return the forecast (in ISO 8601 format, UTC preferred, e.g., 2025-08-11T00:00:00Z).")]	DateTime date,
		CancellationToken cancellationToken = default
		)
    {
		return await dispatcher.Send(new GetAlertRequest(latitude, longitude, date), cancellationToken);
	}
}