using System.Text.Json.Serialization;

namespace WeatherMcpServer.Models;

public record WeatherResponse(
    [property: JsonPropertyName("lat")] double Latitude,
    [property: JsonPropertyName("lon")] double Longitude,
    [property: JsonPropertyName("timezone")] string Timezone,
    [property: JsonPropertyName("timezone_offset")] int TimezoneOffset,
    [property: JsonPropertyName("current")] CurrentWeather? Current,
    [property: JsonPropertyName("minutely")] MinutelyWeather[]? Minutely,
    [property: JsonPropertyName("hourly")] HourlyWeather[]? Hourly,
    [property: JsonPropertyName("daily")] DailyWeather[]? Daily,
    [property: JsonPropertyName("alerts")] WeatherAlert[]? Alerts
);

public record CurrentWeather(
    [property: JsonPropertyName("dt")] long DateTime,
    [property: JsonPropertyName("sunrise")] long? Sunrise,
    [property: JsonPropertyName("sunset")] long? Sunset,
    [property: JsonPropertyName("temp")] double Temperature,
    [property: JsonPropertyName("feels_like")] double FeelsLike,
    [property: JsonPropertyName("pressure")] int Pressure,
    [property: JsonPropertyName("humidity")] int Humidity,
    [property: JsonPropertyName("dew_point")] double DewPoint,
    [property: JsonPropertyName("uvi")] double UvIndex,
    [property: JsonPropertyName("clouds")] int Clouds,
    [property: JsonPropertyName("visibility")] int Visibility,
    [property: JsonPropertyName("wind_speed")] double WindSpeed,
    [property: JsonPropertyName("wind_deg")] int WindDegree,
    [property: JsonPropertyName("wind_gust")] double? WindGust,
    [property: JsonPropertyName("weather")] WeatherCondition[] Weather
);

public record MinutelyWeather(
    [property: JsonPropertyName("dt")] long DateTime,
    [property: JsonPropertyName("precipitation")] double Precipitation
);

public record HourlyWeather(
    [property: JsonPropertyName("dt")] long DateTime,
    [property: JsonPropertyName("temp")] double Temperature,
    [property: JsonPropertyName("feels_like")] double FeelsLike,
    [property: JsonPropertyName("pressure")] int Pressure,
    [property: JsonPropertyName("humidity")] int Humidity,
    [property: JsonPropertyName("dew_point")] double DewPoint,
    [property: JsonPropertyName("uvi")] double UvIndex,
    [property: JsonPropertyName("clouds")] int Clouds,
    [property: JsonPropertyName("visibility")] int Visibility,
    [property: JsonPropertyName("wind_speed")] double WindSpeed,
    [property: JsonPropertyName("wind_deg")] int WindDegree,
    [property: JsonPropertyName("wind_gust")] double? WindGust,
    [property: JsonPropertyName("weather")] WeatherCondition[] Weather,
    [property: JsonPropertyName("pop")] double ProbabilityOfPrecipitation
);

public record DailyWeather(
    [property: JsonPropertyName("dt")] long DateTime,
    [property: JsonPropertyName("sunrise")] long Sunrise,
    [property: JsonPropertyName("sunset")] long Sunset,
    [property: JsonPropertyName("moonrise")] long Moonrise,
    [property: JsonPropertyName("moonset")] long Moonset,
    [property: JsonPropertyName("moon_phase")] double MoonPhase,
    [property: JsonPropertyName("summary")] string Summary,
    [property: JsonPropertyName("temp")] DailyTemperature Temperature,
    [property: JsonPropertyName("feels_like")] DailyFeelsLike FeelsLike,
    [property: JsonPropertyName("pressure")] int Pressure,
    [property: JsonPropertyName("humidity")] int Humidity,
    [property: JsonPropertyName("dew_point")] double DewPoint,
    [property: JsonPropertyName("wind_speed")] double WindSpeed,
    [property: JsonPropertyName("wind_deg")] int WindDegree,
    [property: JsonPropertyName("wind_gust")] double? WindGust,
    [property: JsonPropertyName("weather")] WeatherCondition[] Weather,
    [property: JsonPropertyName("clouds")] int Clouds,
    [property: JsonPropertyName("pop")] double ProbabilityOfPrecipitation,
    [property: JsonPropertyName("rain")] double? Rain,
    [property: JsonPropertyName("snow")] double? Snow,
    [property: JsonPropertyName("uvi")] double UvIndex
);

public record DailyTemperature(
    [property: JsonPropertyName("day")] double Day,
    [property: JsonPropertyName("min")] double Min,
    [property: JsonPropertyName("max")] double Max,
    [property: JsonPropertyName("night")] double Night,
    [property: JsonPropertyName("eve")] double Evening,
    [property: JsonPropertyName("morn")] double Morning
);

public record DailyFeelsLike(
    [property: JsonPropertyName("day")] double Day,
    [property: JsonPropertyName("night")] double Night,
    [property: JsonPropertyName("eve")] double Evening,
    [property: JsonPropertyName("morn")] double Morning
);

public record WeatherCondition(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("main")] string Main,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("icon")] string Icon
);

public record WeatherAlert(
    [property: JsonPropertyName("sender_name")] string SenderName,
    [property: JsonPropertyName("event")] string Event,
    [property: JsonPropertyName("start")] long Start,
    [property: JsonPropertyName("end")] long End,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("tags")] string[] Tags
);

public record GeolocationResponse(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("local_names")] Dictionary<string, string>? LocalNames,
    [property: JsonPropertyName("lat")] double Latitude,
    [property: JsonPropertyName("lon")] double Longitude,
    [property: JsonPropertyName("country")] string Country,
    [property: JsonPropertyName("state")] string? State
);