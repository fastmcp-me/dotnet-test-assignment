using System.Text;
using WeatherMcpServer.Business.Models;

namespace WeatherMcpServer.Business.Infrastructure.Formatters;

public class WeatherResponseFormatter : IWeatherResponseFormatter
{
    public string FormatCurrentWeather(CurrentWeatherData weather)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"📍 Current Weather in {weather.City}, {weather.Country}");
        sb.AppendLine($"🌡️ Temperature: {weather.Temperature}°C (feels like {weather.FeelsLike}°C)");
        sb.AppendLine($"☁️ Condition: {weather.MainCondition} - {weather.Description}");
        sb.AppendLine($"💧 Humidity: {weather.Humidity}%");
        sb.AppendLine($"💨 Wind Speed: {weather.WindSpeed} m/s");
        sb.AppendLine($"🔵 Pressure: {weather.Pressure} hPa");
        sb.AppendLine($"⏰ Last Updated: {weather.Timestamp:yyyy-MM-dd HH:mm:ss}");
        
        return sb.ToString();
    }

    public string FormatWeatherForecast(WeatherForecastData forecast)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"📅 Weather Forecast for {forecast.City}, {forecast.Country}");
        sb.AppendLine();

        foreach (var day in forecast.DailyForecasts)
        {
            sb.AppendLine($"📆 {day.Date:dddd, MMMM d}");
            sb.AppendLine($"   🌡️ Temperature: {day.Temperature}°C (Min: {day.TemperatureMin}°C, Max: {day.TemperatureMax}°C)");
            sb.AppendLine($"   ☁️ Condition: {day.MainCondition} - {day.Description}");
            sb.AppendLine($"   💧 Humidity: {day.Humidity}% | Precipitation: {day.PrecipitationProbability}%");
            sb.AppendLine($"   💨 Wind Speed: {day.WindSpeed} m/s");
            sb.AppendLine();
        }

        return sb.ToString();
    }

    public string FormatWeatherAlerts(WeatherAlertsData alerts)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"⚠️ Weather Alerts for {alerts.City}, {alerts.Country}");
        sb.AppendLine();

        if (alerts.Alerts.Count == 0)
        {
            sb.AppendLine("✅ No active weather alerts for this location.");
            sb.AppendLine();
            sb.AppendLine("ℹ️ Note: Weather alerts require a paid API subscription.");
        }
        else
        {
            foreach (var alert in alerts.Alerts)
            {
                var severityIcon = GetSeverityIcon(alert.Severity);

                sb.AppendLine($"{severityIcon} {alert.Title}");
                sb.AppendLine($"   Severity: {alert.Severity}");
                sb.AppendLine($"   Event: {alert.Event}");
                sb.AppendLine($"   Duration: {alert.StartTime:yyyy-MM-dd HH:mm} - {alert.EndTime:yyyy-MM-dd HH:mm}");
                sb.AppendLine($"   Description: {alert.Description}");
                sb.AppendLine();
            }
        }

        return sb.ToString();
    }

    private static string GetSeverityIcon(AlertSeverity severity)
    {
        return severity switch
        {
            AlertSeverity.Extreme => "🔴",
            AlertSeverity.Severe => "🟠",
            AlertSeverity.Moderate => "🟡",
            AlertSeverity.Minor => "🟢",
            _ => "⚪"
        };
    }
}