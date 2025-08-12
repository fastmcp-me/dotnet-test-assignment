using System.ComponentModel;
using System.Text;
using System.Linq;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using WeatherMcpServer.Services;

namespace WeatherMcpServer.Tools;

public class WeatherTools
{
    private readonly IWeatherService _weatherService;
    private readonly ILogger<WeatherTools> _logger;

    public WeatherTools(IWeatherService weatherService, ILogger<WeatherTools> logger)
    {
        _weatherService = weatherService;
        _logger = logger;
    }

    [McpServerTool]
    [Description("Gets current weather conditions for the specified city.")]
    public async Task<string> GetCurrentWeather(
        [Description("The city name to get weather for")] string city,
        [Description("Optional: Country code (e.g., 'US', 'UK', 'RU')")] string? countryCode = null)
    {
        try
        {
            _logger.LogInformation("Getting current weather for {City}, {CountryCode}", city, countryCode);
            
            var weatherData = await _weatherService.GetCurrentWeatherAsync(city, countryCode);
            
            if (weatherData == null)
            {
                return $"❌ Unable to retrieve weather data for {city}. Please check the city name and try again.";
            }

            var result = new StringBuilder();
            result.AppendLine($"🌤️ **Current Weather in {weatherData.Name}, {weatherData.Sys?.Country}**");
            result.AppendLine();
            
            if (weatherData.Weather?.Length > 0)
            {
                var weather = weatherData.Weather[0];
                result.AppendLine($"**Condition:** {weather.Main} - {weather.Description}");
            }

            if (weatherData.Main != null)
            {
                result.AppendLine($"**Temperature:** {weatherData.Main.Temp:F1}°C (feels like {weatherData.Main.FeelsLike:F1}°C)");
                result.AppendLine($"**Min/Max:** {weatherData.Main.TempMin:F1}°C / {weatherData.Main.TempMax:F1}°C");
                result.AppendLine($"**Humidity:** {weatherData.Main.Humidity}%");
                result.AppendLine($"**Pressure:** {weatherData.Main.Pressure} hPa");
            }

            if (weatherData.Wind != null)
            {
                result.AppendLine($"**Wind:** {weatherData.Wind.Speed} m/s at {weatherData.Wind.Deg}°");
                if (weatherData.Wind.Gust.HasValue)
                {
                    result.AppendLine($"**Wind Gusts:** {weatherData.Wind.Gust:F1} m/s");
                }
            }

            if (weatherData.Clouds != null)
            {
                result.AppendLine($"**Cloudiness:** {weatherData.Clouds.All}%");
            }

            result.AppendLine($"**Visibility:** {weatherData.Visibility / 1000.0:F1} km");
            
            var dateTime = DateTimeOffset.FromUnixTimeSeconds(weatherData.Dt);
            result.AppendLine($"**Last Updated:** {dateTime:yyyy-MM-dd HH:mm} UTC");

            return result.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current weather for {City}", city);
            return $"❌ An error occurred while retrieving weather data for {city}. Please try again later.";
        }
    }

    [McpServerTool]
    [Description("Gets weather forecast for the specified city (up to 5 days).")]
    public async Task<string> GetWeatherForecast(
        [Description("The city name to get weather forecast for")] string city,
        [Description("Optional: Country code (e.g., 'US', 'UK', 'RU')")] string? countryCode = null,
        [Description("Number of days for forecast (1-5, default: 3)")] int days = 3)
    {
        try
        {
            days = Math.Max(1, Math.Min(5, days)); // Ensure days is between 1 and 5
            
            _logger.LogInformation("Getting {Days}-day weather forecast for {City}, {CountryCode}", days, city, countryCode);
            
            var forecastData = await _weatherService.GetWeatherForecastAsync(city, countryCode, days);
            
            if (forecastData?.List == null || forecastData.List.Length == 0)
            {
                return $"❌ Unable to retrieve weather forecast for {city}. Please check the city name and try again.";
            }

            var result = new StringBuilder();
            result.AppendLine($"📅 **{days}-Day Weather Forecast for {forecastData.City?.Name}, {forecastData.City?.Country}**");
            result.AppendLine();

            // Group forecast by day
            var dailyForecasts = forecastData.List
                .GroupBy(f => DateTimeOffset.FromUnixTimeSeconds(f.Dt).Date)
                .Take(days)
                .ToList();

            foreach (var dayGroup in dailyForecasts)
            {
                var date = dayGroup.Key;
                var dayForecasts = dayGroup.ToList();
                
                result.AppendLine($"**{date:dddd, MMMM dd}**");
                
                // Get representative forecast (midday if available, otherwise first)
                var mainForecast = dayForecasts
                    .OrderBy(f => Math.Abs(DateTimeOffset.FromUnixTimeSeconds(f.Dt).Hour - 12))
                    .First();

                if (mainForecast.Weather?.Length > 0)
                {
                    var weather = mainForecast.Weather[0];
                    result.AppendLine($"  🌤️ {weather.Main} - {weather.Description}");
                }

                if (mainForecast.Main != null)
                {
                    var tempMin = dayForecasts.Min(f => f.Main?.TempMin ?? double.MaxValue);
                    var tempMax = dayForecasts.Max(f => f.Main?.TempMax ?? double.MinValue);
                    
                    result.AppendLine($"  🌡️ {tempMin:F1}°C to {tempMax:F1}°C");
                    result.AppendLine($"  💧 Humidity: {mainForecast.Main.Humidity}%");
                }

                if (mainForecast.Wind != null)
                {
                    result.AppendLine($"  💨 Wind: {mainForecast.Wind.Speed:F1} m/s");
                }

                // Show precipitation probability if available
                if (mainForecast.Pop > 0)
                {
                    result.AppendLine($"  🌧️ Precipitation: {mainForecast.Pop * 100:F0}%");
                }

                result.AppendLine();
            }

            return result.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting weather forecast for {City}", city);
            return $"❌ An error occurred while retrieving weather forecast for {city}. Please try again later.";
        }
    }

    [McpServerTool]
    [Description("Gets weather alerts for the specified city (requires coordinates).")]
    public async Task<string> GetWeatherAlerts(
        [Description("The city name to get weather alerts for")] string city,
        [Description("Optional: Country code (e.g., 'US', 'UK', 'RU')")] string? countryCode = null)
    {
        try
        {
            _logger.LogInformation("Getting weather alerts for {City}, {CountryCode}", city, countryCode);
            
            // First get current weather to obtain coordinates
            var weatherData = await _weatherService.GetCurrentWeatherAsync(city, countryCode);
            
            if (weatherData?.Coord == null)
            {
                return $"❌ Unable to retrieve coordinates for {city}. Please check the city name and try again.";
            }

            var alertsData = await _weatherService.GetWeatherAlertsAsync(weatherData.Coord.Lat, weatherData.Coord.Lon);
            
            if (alertsData?.Alerts == null || alertsData.Alerts.Length == 0)
            {
                return $"✅ **No Weather Alerts for {weatherData.Name}, {weatherData.Sys?.Country}**\n\nThere are currently no active weather alerts for this location.";
            }

            var result = new StringBuilder();
            result.AppendLine($"⚠️ **Weather Alerts for {weatherData.Name}, {weatherData.Sys?.Country}**");
            result.AppendLine();

            foreach (var alert in alertsData.Alerts)
            {
                result.AppendLine($"**🚨 {alert.Event}**");
                if (!string.IsNullOrEmpty(alert.SenderName))
                {
                    result.AppendLine($"*Issued by: {alert.SenderName}*");
                }
                
                var startTime = DateTimeOffset.FromUnixTimeSeconds(alert.Start);
                var endTime = DateTimeOffset.FromUnixTimeSeconds(alert.End);
                result.AppendLine($"**Period:** {startTime:MMM dd, HH:mm} - {endTime:MMM dd, HH:mm} UTC");
                
                if (!string.IsNullOrEmpty(alert.Description))
                {
                    result.AppendLine($"**Description:** {alert.Description}");
                }
                
                if (alert.Tags?.Length > 0)
                {
                    result.AppendLine($"**Tags:** {string.Join(", ", alert.Tags)}");
                }
                
                result.AppendLine();
            }

            return result.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting weather alerts for {City}", city);
            return $"❌ An error occurred while retrieving weather alerts for {city}. Please try again later.";
        }
    }
}