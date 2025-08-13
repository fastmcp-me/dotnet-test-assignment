namespace WeatherMcpServer.Infrastructure.OpenWeather;

public class GeocodingResponseDto
{
    public string Name { get; set; } = string.Empty;
    public LocalNamesDto? Local_Names { get; set; }
    public double Lat { get; set; }
    public double Lon { get; set; }
    public string Country { get; set; } = string.Empty;
    public string? State { get; set; }
}

public class LocalNamesDto
{
    public string? Ru { get; set; }
    public string? En { get; set; }
}