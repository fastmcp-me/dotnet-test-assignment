using System.Text.Json;
using WeatherMcpServer.Extensions;

namespace WeatherMcpServer.Formatters;

public static class OpenWeatherFormatter
{
    public static string ToCurrentWeatherDescription(this JsonElement currentWeatherRoot)
    {
        var date = currentWeatherRoot.GetProperty("dt").ToDateTimeString();

        string description = currentWeatherRoot.GetProperty("weather")[0].GetProperty("description").GetString() ?? "N/A";

        double temp = currentWeatherRoot.GetProperty("main").GetProperty("temp").GetDouble();
        int pressure = currentWeatherRoot.GetProperty("main").GetProperty("pressure").GetInt32();

        return $"{date}: {description}, temperature: {temp}°C, pressure: {pressure} hPa.";
    }

    public static string ToDailyForecastDescription(this JsonElement forecastRoot, int hourStep)
    {
        var list = forecastRoot.GetProperty("list");

        if (list.GetArrayLength() == 0)
            return $"No forecast data available.";

        var dailyDescriptions = new List<string>();
        int firstRecordTime = 0;
        bool isFirstRecord = true;

        foreach (var item in list.EnumerateArray())
        {
            var recordTime = item.GetProperty("dt").GetInt64().ToDateTime().Hour;

            if (isFirstRecord)
            {
                dailyDescriptions.Add(item.ToCurrentWeatherDescription());
                firstRecordTime = recordTime;
                isFirstRecord = false;
            }
            else
                if (recordTime == firstRecordTime - hourStep)
                    dailyDescriptions.Add(item.ToCurrentWeatherDescription());
        }

        return string.Join("\n", dailyDescriptions);
    }

    public static string ToDateTimeString(this JsonElement dt) => 
        dt.GetInt64().ToDateTime().ToString("dd-MM-yyyy HH:mm");

    public static string ToAlertsDescription(this JsonElement alertRoot)
    {
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
}
