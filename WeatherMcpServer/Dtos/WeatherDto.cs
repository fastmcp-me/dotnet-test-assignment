using System.Text;
using Newtonsoft.Json;

namespace WeatherMcpServer.Dtos;

public class WeatherDto
{
    public CurrentWeatherDto Current { get; set; } = new CurrentWeatherDto();
    public List<WeatherForecastDescriptionDto> Daily { get; set; } = new List<WeatherForecastDescriptionDto>();

    public List<WeatherAlertsDto> Alerts { get; set; } = new List<WeatherAlertsDto>();
}

public class CurrentWeatherDto
{
    public int? Sunrise { get; set; }
    public int? Sunset { get; set; }
    public double? Temp { get; set; }
    
    [JsonProperty("feels_like")]
    public double? FeelsLike { get; set; }
    
    public double? Humidity { get; set; }
    public double? Uvi { get; set; }
    public double? Clouds { get; set; }
    public int? Visibility { get; set; }
   
    [JsonProperty("wind_speed")]
    public double? WindSpeed { get; set; }
   
    public List<WeatherDescriptionDto> Weather { get; set; }

    public string ToFormattedString(string locationName)
    {
        var mainWeather = Weather?.FirstOrDefault();
        var result = new StringBuilder();
        
        result.AppendLine($"Current Weather in {locationName}:");
        result.AppendLine($"├─ Condition: {mainWeather?.Main ?? "Unknown"} - {mainWeather?.Description ?? "No description"}");
        result.AppendLine($"├─ Temperature: {Temp?.ToString("F1") ?? "N/A"}°C");
        result.AppendLine($"├─ Feels Like: {FeelsLike?.ToString("F1") ?? "N/A"}°C");
        result.AppendLine($"├─ Humidity: {Humidity?.ToString("F0") ?? "N/A"}%");
        result.AppendLine($"├─ Wind Speed: {WindSpeed?.ToString("F1") ?? "N/A"} m/s");
        result.AppendLine($"├─ UV Index: {Uvi?.ToString("F1") ?? "N/A"}");
        result.AppendLine($"├─ Cloud Cover: {Clouds?.ToString("F0") ?? "N/A"}%");
        result.AppendLine($"└─ Visibility: {(Visibility.HasValue ? $"{Visibility / 1000.0:F1} km" : "N/A")}");
        
        if (Sunrise.HasValue && Sunset.HasValue)
        {
            var sunrise = DateTimeOffset.FromUnixTimeSeconds(Sunrise.Value).ToString("HH:mm");
            var sunset = DateTimeOffset.FromUnixTimeSeconds(Sunset.Value).ToString("HH:mm");
            result.AppendLine($"Sun Times: {sunrise} - {sunset}");
        }
        
        return result.ToString();
    }

    public string ToSummaryString()
    {
        var mainWeather = Weather?.FirstOrDefault();
        return $"   {mainWeather?.Description ?? "Unknown"} - {Temp?.ToString("F1") ?? "N/A"}°C (feels like {FeelsLike?.ToString("F1") ?? "N/A"}°C)";
    }
}

public class WeatherDescriptionDto
{ 
    public string? Main { get; set; }
    public string? Description { get; set; }
}

public class WeatherForecastDescriptionDto
{ 
    public int? Dt { get; set; }
    public int? Sunrise { get; set; }
    public int? Sunset { get; set; }
    public string? Summary { get; set; }

    [JsonProperty("temp")]
    public TemperatureDto? Temperature { get; set; }
    
    [JsonProperty("feels_like")]
    public TemperatureDto? FeelsLike { get; set; }
   
    public List<WeatherDescriptionDto> Weather { get; set; }
    public double Clouds { get; set; }
    public double? Rain { get; set; }

    public string ToFormattedString()
    {
        var date = Dt.HasValue 
            ? DateTimeOffset.FromUnixTimeSeconds(Dt.Value).ToString("MMM dd, yyyy (dddd)")
            : "Unknown date";
            
        var condition = Weather?.FirstOrDefault()?.Description ?? "Unknown condition";
        var temp = Temperature;
        
        var result = new StringBuilder();
        result.AppendLine($"📅 {date}");
        result.AppendLine($"   Weather: {condition}");
        
        if (temp != null)
        {
            result.AppendLine($"   Temperature: {temp.Min.ToString() ?? "N/A"}°C - {temp.Max.ToString() ?? "N/A"}°C");
            if (!string.IsNullOrEmpty(temp.Day.ToString()))
                result.AppendLine($"   Day: {temp.Day.ToString()}°C, Night: {temp.Night.ToString() ?? "N/A"}°C");
        }
        
        if (Rain.HasValue && Rain > 0)
            result.AppendLine($"   Rain: {Rain:F1}mm");
            
        result.AppendLine($"   Cloud Cover: {Clouds:F0}%");
        
        if (!string.IsNullOrEmpty(Summary))
            result.AppendLine($"   Summary: {Summary}");
            
        return result.ToString();
    }

    public string ToSummaryString()
    {
        var date = Dt.HasValue 
            ? DateTimeOffset.FromUnixTimeSeconds(Dt.Value).ToString("MMM dd")
            : "Unknown";
        var condition = Weather?.FirstOrDefault()?.Description ?? "Unknown";
        var temp = Temperature;
        return $"{date}: {condition}, {temp?.Min.ToString() ?? "N/A"}°C - {temp?.Max.ToString()?? "N/A"}°C";
    }
}

public class TemperatureDto
{
    public double Day { get; set; }
    public double Min { get; set; }
    public double Max { get; set; }
    public double Night { get; set; }
    public double Eve { get; set; }
    public double Morn { get; set; }
}

public class WeatherAlertsDto
{
    [JsonProperty("sender_name")]
    public string? SenderName { get; set; }
    
    public string? Event { get; set; }
    public int? Start { get; set; }
    public int? End { get; set; }
    public string? Description { get; set; }
    public List<string> Tags { get; set; } = new List<string>();

    public string ToFormattedString(int alertNumber)
    {
        var result = new StringBuilder();
        result.AppendLine($"🚨 Alert #{alertNumber}: {Event ?? "Unknown Event"}");
        result.AppendLine($"   Sender: {SenderName ?? "Unknown"}");
        
        if (Start.HasValue)
        {
            var startTime = DateTimeOffset.FromUnixTimeSeconds(Start.Value).ToString("MMM dd, yyyy HH:mm");
            result.AppendLine($"   Start: {startTime}");
        }
        
        if (End.HasValue)
        {
            var endTime = DateTimeOffset.FromUnixTimeSeconds(End.Value).ToString("MMM dd, yyyy HH:mm");
            result.AppendLine($"   End: {endTime}");
        }
        
        if (!string.IsNullOrEmpty(Description))
        {
            result.AppendLine($"   Description: {Description}");
        }
        
        if (Tags != null && Tags.Any())
        {
            result.AppendLine($"   Tags: {string.Join(", ", Tags)}");
        }
        
        return result.ToString();
    }
}

