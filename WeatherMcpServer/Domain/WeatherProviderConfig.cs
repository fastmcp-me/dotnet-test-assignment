namespace WeatherMcpServer.Domain;

public class WeatherProviderConfig
{
    public string ApiKeyEnvVar { get; set; } = null!;

    public string ApiKey { get; set; } = null!;

    public string BaseUrl { get; set; } = null!;

    public int Weight { get; set; } = 0;

    public int Priority { get; set; }

}
