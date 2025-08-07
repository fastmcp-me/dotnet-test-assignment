using System.Text.Json;
using System.Text.Json.Serialization;

namespace WeatherMcpServer.Features.Services.DTOs.OpenWeatherMap.WeatherForecast;

/// <summary>
/// Converts the JSON response from the OpenWeatherMap API to a <see cref="WeatherForecastDto"/> object.
/// </summary>
internal class WeatherForecastJsonConverter : JsonConverter<WeatherForecastDto>
{
    /// <summary>
    /// Reads and converts the JSON to create a <see cref="WeatherForecastDto"/> object.
    /// </summary>
    public override WeatherForecastDto? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        var cityElement = root.GetProperty("city");

        var result = new WeatherForecastDto
        {
            City = cityElement.GetProperty("name").GetString() ?? "",
            Country = cityElement.GetProperty("country").GetString() ?? "",
        };

        if (root.TryGetProperty("list", out var forecastList))
        {
            var dailyForecasts = new Dictionary<DateOnly, List<(double Temp, string Description)>>();

            foreach (var item in forecastList.EnumerateArray())
            {
                var timestamp = item.GetProperty("dt").GetInt64();
                var dateTime = DateTimeOffset.FromUnixTimeSeconds(timestamp);
                var date = DateOnly.FromDateTime(dateTime.DateTime);

                var temp = item.GetProperty("main").GetProperty("temp").GetDouble();
                var description = item.GetProperty("weather")[0].GetProperty("description").GetString() ?? "";

                if (!dailyForecasts.TryGetValue(date, out List<(double Temp, string Description)>? value))
                {
                    value = [];
                    dailyForecasts[date] = value;
                }

                value.Add((temp, description));
            }

            result.Forecast = [.. dailyForecasts
                .OrderBy(day => day.Key)
                .Select(day => new DailyForecast
                {
                    Date = day.Key,
                    MinTemperature = day.Value.Min(x => x.Temp),
                    MaxTemperature = day.Value.Max(x => x.Temp),
                    Description = day.Value
                        .GroupBy(f => f.Description)
                        .OrderByDescending(g => g.Count())
                        .First()
                        .Key,
                })];
        }

        return result;
    }

    /// <summary>
    /// This method is not implemented and will throw a <see cref="NotImplementedException"/>.
    /// </summary>
    public override void Write(Utf8JsonWriter writer, WeatherForecastDto value, JsonSerializerOptions options)
    {
        throw new NotImplementedException("Serialization is not supported.");
    }
}