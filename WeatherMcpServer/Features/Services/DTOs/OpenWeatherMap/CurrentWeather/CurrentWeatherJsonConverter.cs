using System.Text.Json;
using System.Text.Json.Serialization;

namespace WeatherMcpServer.Features.Services.DTOs.OpenWeatherMap.CurrentWeather;

/// <summary>
/// Converts the JSON response from the OpenWeatherMap API to a <see cref="CurrentWeatherDto"/> object.
/// </summary>
internal class CurrentWeatherJsonConverter : JsonConverter<CurrentWeatherDto>
{
    /// <summary>
    /// Reads and converts the JSON to create a <see cref="CurrentWeatherDto"/> object.
    /// </summary>
    public override CurrentWeatherDto? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;
        var main = root.GetProperty("main");

        return new CurrentWeatherDto
        {
            City = root.GetProperty("name").GetString() ?? "",
            Country = root.GetProperty("sys").GetProperty("country").GetString() ?? "",
            Temperature = main.GetProperty("temp").GetDouble(),
            TemperatureFeelsLike = main.GetProperty("feels_like").GetDouble(),
            Description = root.GetProperty("weather")[0].GetProperty("description").GetString() ?? "",
            WindSpeed = root.GetProperty("wind").GetProperty("speed").GetDouble(),
            Humidity = main.GetProperty("humidity").GetInt32(),
            Pressure = main.GetProperty("pressure").GetInt32(),
        };
    }

    /// <summary>
    /// This method is not implemented and will throw a <see cref="NotImplementedException"/>.
    /// </summary>
    public override void Write(Utf8JsonWriter writer, CurrentWeatherDto value, JsonSerializerOptions options)
    {
        throw new NotImplementedException("Serialization is not supported.");
    }
}
