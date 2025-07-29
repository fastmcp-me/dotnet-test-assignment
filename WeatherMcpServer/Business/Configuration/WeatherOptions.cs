namespace WeatherMcpServer.Business.Configuration;

public class WeatherOptions
{
    public const string SectionName = "Weather";
    
    public string[] Choices { get; set; } = Array.Empty<string>();
    public string DefaultChoices { get; set; } = "balmy,rainy,stormy";
}