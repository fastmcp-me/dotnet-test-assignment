using System.Text;
using WeatherMcpServer.Dtos;

namespace WeatherMcpServer.Formatters;

public static class WeatherFormatter
{
    public static string FormatCurrentWeather(CurrentWeatherDto current, string locationName)
    {
        return current.ToFormattedString(locationName);
    }

    public static string FormatForecast(List<WeatherForecastDescriptionDto> daily, string locationName)
    {
        var result = new StringBuilder();
        result.AppendLine($"Weather Forecast for {locationName}:");
        
        if (daily == null || !daily.Any())
        {
            result.AppendLine("No forecast data available.");
            return result.ToString();
        }
        
        
        foreach (var day in daily)
        {
            result.AppendLine($"\n{day.ToFormattedString()}");
        }
        
        return result.ToString();
    }

    public static string FormatAlerts(List<WeatherAlertsDto> alerts, string locationName)
    {
        var result = new StringBuilder();
        result.AppendLine($"Weather Alerts for {locationName}:");
        
        if (alerts == null || !alerts.Any())
        {
            result.AppendLine("No active weather alerts.");
            return result.ToString();
        }
        
        for (int i = 0; i < alerts.Count; i++)
        {
            result.AppendLine($"\n{alerts[i].ToFormattedString(i + 1)}");
        }
        
        return result.ToString();
    }

}