
namespace WeatherMcpServer.Infrastructure.OpenWeather;

public class OneCallForecastDto
{
    public List<DailyDto> Daily { get; set; }
}

public class DailyDto
{
    public long Dt { get; set; }
    public TempDto Temp { get; set; }
    public FeelsLikeDto Feels_Like { get; set; }
    public int Humidity { get; set; }
    public List<WeatherDescriptionDto> Weather { get; set; }
}

public class TempDto
{
    public decimal Day { get; set; }
}

public class FeelsLikeDto
{
    public decimal Day { get; set; }
}

public class WeatherDescriptionDto
{
    public string Description { get; set; }
}
