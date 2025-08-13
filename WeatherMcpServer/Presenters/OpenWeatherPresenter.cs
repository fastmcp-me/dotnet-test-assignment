using Json.Schema;
using System.Text.Json;
using WeatherMcpServer.Model;

namespace WeatherMcpServer.Presenters;

public static class OpenWeatherPresenter
{
    private static readonly JsonSchema? _geoSchema;

    static OpenWeatherPresenter()
    {
        _geoSchema = JsonSchema.FromFile(Path.Combine(AppContext.BaseDirectory, "Schemas/OpenWeather/geo-direct.json"));
    }

    public static GeoCoordinate GetGeoCoordinate(this JsonElement geoRoot)
    {
        if (_geoSchema!.Evaluate(geoRoot).IsValid)
        {
            if (geoRoot.GetArrayLength() == 0)
                throw new ArgumentException("Location not found. Please check the city and country code.");

            var latitude = geoRoot[0].GetProperty("lat").GetDouble();
            var longitude = geoRoot[0].GetProperty("lon").GetDouble();

            return new GeoCoordinate
            {
                Latitude = latitude,
                Longitude = longitude
            };
        }

        throw new ArgumentException("Invalid geo data format.");
    }

    public static string GetErrorMessage(this JsonElement content)
    {
        if (content.TryGetProperty("message", out var messageElement))
        {
            return messageElement.GetString() ?? "No error message provided.";
        }

        return "No 'message' field in response.";
    }
}
