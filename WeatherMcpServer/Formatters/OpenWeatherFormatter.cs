using Json.Schema;
using System.Text.Json;
using WeatherMcpServer.Extensions;

namespace WeatherMcpServer.Formatters;

public static class OpenWeatherFormatter
{
    private static readonly JsonSchema _currentWeatherSchema = JsonSchema.FromFile("Schemas/OpenWeather/current-weather.json");

    public static string ToCurrentWeatherDescription(this JsonElement currentWeatherRoot)
    {
        if (!_currentWeatherSchema.Evaluate(currentWeatherRoot).IsValid)
            return "Invalid current weather data format.";

        var date = currentWeatherRoot.GetProperty("dt").ToDateTimeString();
        string description = currentWeatherRoot.GetProperty("weather")[0].GetProperty("description").GetString() ?? "N/A";
        double temp = currentWeatherRoot.GetProperty("main").GetProperty("temp").GetDouble();
        int pressure = currentWeatherRoot.GetProperty("main").GetProperty("pressure").GetInt32();

        return $"{date}: {description}, temperature: {temp}°C, pressure: {pressure} hPa.";
    }

    private static readonly JsonSchema _forecastWeatherSchema = JsonSchema.FromFile("Schemas/OpenWeather/forecast-5day-3hour.json");

    public static string ToDailyForecastDescription(this JsonElement forecastRoot, int hourStep)
    {
        if (!_forecastWeatherSchema.Evaluate(forecastRoot).IsValid)
            return "Invalid forecast data format.";

        var list = forecastRoot.GetProperty("list");

        if (list.GetArrayLength() == 0)
            return "No forecast data available.";

        var dailyDescriptions = new List<string>();
        int firstRecordTime = 0;
        bool isFirstRecord = true;

        foreach (var item in list.EnumerateArray())
        {
            var recordTime = item.GetProperty("dt").GetInt64().ToDateTime().Hour;

            if (isFirstRecord)
            {
                dailyDescriptions.Add(item.ToForecastItemDescription());
                firstRecordTime = recordTime;
                isFirstRecord = false;
            }
            else
                if (recordTime == firstRecordTime - hourStep)
                    dailyDescriptions.Add(item.ToForecastItemDescription());
        }

        return string.Join("\n", dailyDescriptions);
    }

    private static readonly JsonSchema _forecastItemSchema = JsonSchema.FromFile("Schemas/OpenWeather/forecast-item.json");

    private static string ToForecastItemDescription(this JsonElement forecastItem)
    {
        if (!_forecastItemSchema.Evaluate(forecastItem).IsValid)
            return "Invalid forecast item data format.";

        var date = forecastItem.GetProperty("dt").ToDateTimeString();
        string description = forecastItem.GetProperty("weather")[0].GetProperty("description").GetString() ?? "N/A";
        double temp = forecastItem.GetProperty("main").GetProperty("temp").GetDouble();
        int pressure = forecastItem.GetProperty("main").GetProperty("pressure").GetInt32();

        return $"{date}: {description}, temperature: {temp}°C, pressure: {pressure} hPa.";
    }

    private static readonly JsonSchema _alertsSchema = JsonSchema.FromFile("Schemas/OpenWeather/weather-alerts.json");

    public static string ToAlertsDescription(this JsonElement alertRoot)
    {
        if (!_alertsSchema.Evaluate(alertRoot).IsValid)
            return "Invalid weather alerts data format.";

        if (!alertRoot.TryGetProperty("alerts", out var alerts) || alerts.GetArrayLength() == 0)
            return "No weather alerts for this location.";

        var result = new List<string>();
        foreach (var alert in alerts.EnumerateArray())
        {
            var sender = alert.GetProperty("sender_name").GetString() ?? "Unknown sender";
            var description = alert.GetProperty("description").GetString() ?? "No description";
            result.Add($"Sender: {sender}\nDescription: {description}");
        }
        return string.Join("\n", result);
    }

    private static string ToDateTimeString(this JsonElement dt) =>
        dt.GetInt64().ToDateTime().ToString("dd-MM-yyyy HH:mm");
}
