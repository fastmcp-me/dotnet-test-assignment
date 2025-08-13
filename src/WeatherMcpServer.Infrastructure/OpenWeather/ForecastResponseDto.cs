

namespace WeatherMcpServer.Infrastructure.OpenWeather;

public class ForecastResponseDto
{
    public List<ForecastItemDto> List { get; set; } = [];
}

public class ForecastItemDto
{
    public long Dt { get; set; }
    public MainDto Main { get; set; }
    public List<WeatherDto> Weather { get; set; } = [];
}

public class MainDto
{
    public double Temp { get; set; }
}

public class WeatherDto
{
    public string Main { get; set; }
    public string Description { get; set; }
}
