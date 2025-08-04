namespace Infrastructure.Services.OpenWeatherMap.Models;

public class OW_Forecast
{
    public List<OW_ForecastItem> List { get; set; } = default!;
}