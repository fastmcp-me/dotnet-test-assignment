using System.Threading;

namespace Domain.Common.DTOs.OpenWeatherMap;

public class ForecastDto
{
    public float TempC { get; set; }
    public string Description { get; set; }
    public DateTimeOffset Date { get; set; }

    public ForecastDto(float tempC, string description, DateTimeOffset date)
    {
        TempC = tempC;
        Description = description;
        Date = date;
    }
}