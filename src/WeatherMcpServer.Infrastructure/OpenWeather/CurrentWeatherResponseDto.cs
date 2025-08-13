namespace WeatherMcpServer.Infrastructure.OpenWeather;

public sealed class CurrentWeatherResponseDto
{
    public List<WeatherDto> Weather { get; set; } = [];
    public MainDto Main { get; set; } = new();

    public sealed class WeatherDto
    {
        public string Description { get; set; } = string.Empty;
    }

    public sealed class MainDto
    {
        public decimal Temp { get; set; }
        public decimal Feels_Like { get; set; }
        public int Humidity { get; set; }
    }
}
