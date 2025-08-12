using Weather.Core;
using Weather.Core.Exceptions;
using Weather.Core.Interfaces;
using Weather.Core.Dto;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace Weather.Infrastructure;

internal class WeatherApi(IOptions<WeatherConfiguration> options, HttpClient httpClient, ILogger<WeatherApi> logger) : IWeatherForecast
{
	private WeatherConfiguration Options => options.Value;

	private Dictionary<string, string?> DefaultQueryParams(string city, string? countryCode = null, string? stateCode = null) => new()
	{
		{ "q", countryCode is null ? city : string.Join(",", (new string?[] { city, stateCode, countryCode }).OfType<string>()) },
		{ "appid", Options.ApiKey },
		{ "units", "metric" }
	};

	public async Task<WeatherDto> GetCurrentWeatherByCityAsync(string city, string? countryCode = null, string? stateCode = null, CancellationToken cancellationToken = default)
	{
		logger.LogInformation(QueryHelpers.AddQueryString("weather", DefaultQueryParams(city, countryCode, stateCode)));
		var response = await httpClient.GetAsync(
			QueryHelpers.AddQueryString("weather", DefaultQueryParams(city, countryCode, stateCode)),
			cancellationToken: cancellationToken
		);

		if (!response.IsSuccessStatusCode)
		{
			throw new WeatherArgumentException(
				$"Failed to get weather for {city}. Status code: {response.StatusCode}"
			);
		}

		using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

		return new WeatherDto(
			City: city,
			Temperature: doc.RootElement.GetProperty("main").GetProperty("temp").GetDouble(),
			WeatherCondition: doc.RootElement.GetProperty("weather")[0].GetProperty("description").GetString()!,
			Timestamp: DateTime.Now
		);
	}

	public async Task<WeatherDto> GetWeatherForecastByCityAsync(string city, DateTime date, string? countryCode = null, string? stateCode = null, CancellationToken cancellationToken = default)
	{
		var utcDate = date.ToUniversalTime().Date;

		var queryParams = DefaultQueryParams(city, countryCode, stateCode);
		queryParams["cnt"] = "16"; // A number of days, which will be returned in the API response

		var response = await httpClient.GetAsync(
			QueryHelpers.AddQueryString("forecast", queryParams),
			cancellationToken: cancellationToken
		);

		if (!response.IsSuccessStatusCode)
		{
			throw new WeatherArgumentException(
				$"Failed to get weather for {city}. Status code: {response.StatusCode}"
			);
		}

		using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

		foreach (var day in doc.RootElement.GetProperty("list").EnumerateArray())
		{
			var dt = DateTimeOffset.FromUnixTimeSeconds(day.GetProperty("dt").GetInt64()).UtcDateTime.Date;
			if (dt == utcDate)
			{
				return new WeatherDto(
					City: city,
					Temperature: day.GetProperty("main").GetProperty("temp").GetDouble(),
					WeatherCondition: day.GetProperty("weather")[0].GetProperty("description").GetString()!,
					Timestamp: date
				);
			}
		}


		throw new WeatherArgumentException(
			$"Didn't find weather forecast for {city} on {date:yyyy-MM-dd}"
		);
	}

	public async Task<AlertDto> GetWeatherAlertAsync(double lat, double lon, DateTime date, CancellationToken cancellationToken = default)
	{
		var dt = ((DateTimeOffset)date.ToUniversalTime()).ToUnixTimeSeconds();
		Dictionary<string, string?> query = new()
		{
			{ "lat", lat.ToString() },
			{ "lon", lon.ToString() },
			{ "appid", Options.ApiKey },
			{ "dt", dt.ToString() },
		};

		var content = new StringContent(
			JsonSerializer.Serialize(new { track = new[] { new { lat, lon, dt } } }),
			Encoding.UTF8,
			"application/json"
		);

		var response = await httpClient.PostAsync(
			QueryHelpers.AddQueryString("roadrisk", query),
			content,
			cancellationToken: cancellationToken
		);

		if (!response.IsSuccessStatusCode)
		{
			throw new WeatherArgumentException(
				$"Failed to get weather alert for coordinates ({lat}, {lon}). Status code: {response.StatusCode}"
			);
		}

		using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

		var root = doc.RootElement[0];
		var alerts = root.GetProperty("alerts")[0];

		return new AlertDto(
			Temperature: root.GetProperty("weather").GetProperty("temp").GetDouble(),
			Event: alerts.GetProperty("event").ToString()!,
			AlertLevel: AlertLevel.FromValue(alerts.GetProperty("event_level").GetInt32())
		);
	}
}
