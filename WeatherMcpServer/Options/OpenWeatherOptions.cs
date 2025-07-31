using System.ComponentModel.DataAnnotations;

namespace WeatherMcpServer.Options;

public sealed record class OpenWeatherOptions
{
    [Required]
    public string BaseUrl { get; init; } = string.Empty;

    [Required]
    public string ApiKey { get; init; } = string.Empty;
}
