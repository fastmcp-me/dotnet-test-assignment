namespace Domain.Common.DTOs.OpenWeatherMap;

public class WeatherDto
{
    public float TempC { get; set; }
    public string Description { get; set; }
    public DateTimeOffset TimeUtc { get; set; }

    public WeatherDto(float tempC, string description, DateTimeOffset timeUtc)
    {
        TempC = tempC;
        Description = description;
        TimeUtc = timeUtc;
    }
}