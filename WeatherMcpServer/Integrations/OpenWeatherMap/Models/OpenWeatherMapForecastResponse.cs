using System.Text.Json.Serialization;

namespace WeatherMcpServer.Integrations.OpenWeatherMap.Models;

public class OpenWeatherMapForecastResponse
{
    [JsonPropertyName("cod")]
    public string Cod { get; set; } = string.Empty;
    
    [JsonPropertyName("message")]
    public int Message { get; set; }
    
    [JsonPropertyName("cnt")]
    public int Cnt { get; set; }
    
    [JsonPropertyName("list")]
    public List<ForecastItem> List { get; set; } = new();
    
    [JsonPropertyName("city")]
    public City City { get; set; } = new();
}

public class ForecastItem
{
    [JsonPropertyName("dt")]
    public long Dt { get; set; }
    
    [JsonPropertyName("main")]
    public Main Main { get; set; } = new();
    
    [JsonPropertyName("weather")]
    public List<Weather> Weather { get; set; } = new();
    
    [JsonPropertyName("clouds")]
    public Clouds Clouds { get; set; } = new();
    
    [JsonPropertyName("wind")]
    public Wind Wind { get; set; } = new();
    
    [JsonPropertyName("visibility")]
    public int Visibility { get; set; }
    
    [JsonPropertyName("pop")]
    public double Pop { get; set; }
    
    [JsonPropertyName("sys")]
    public ForecastSys Sys { get; set; } = new();
    
    [JsonPropertyName("dt_txt")]
    public string DtTxt { get; set; } = string.Empty;
}

public class ForecastSys
{
    [JsonPropertyName("pod")]
    public string Pod { get; set; } = string.Empty;
}

public class City
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("coord")]
    public Coordinates Coord { get; set; } = new();
    
    [JsonPropertyName("country")]
    public string Country { get; set; } = string.Empty;
    
    [JsonPropertyName("population")]
    public int Population { get; set; }
    
    [JsonPropertyName("timezone")]
    public int Timezone { get; set; }
    
    [JsonPropertyName("sunrise")]
    public long Sunrise { get; set; }
    
    [JsonPropertyName("sunset")]
    public long Sunset { get; set; }
}