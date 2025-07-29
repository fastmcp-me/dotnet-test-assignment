using System.Text.Json.Serialization;

namespace WeatherMcpServer.Integrations.OpenWeatherMap.Models;

public class OpenWeatherMapCurrentResponse
{
    [JsonPropertyName("coord")]
    public Coordinates Coord { get; set; } = new();
    
    [JsonPropertyName("weather")]
    public List<Weather> Weather { get; set; } = new();
    
    [JsonPropertyName("base")]
    public string Base { get; set; } = string.Empty;
    
    [JsonPropertyName("main")]
    public Main Main { get; set; } = new();
    
    [JsonPropertyName("visibility")]
    public int Visibility { get; set; }
    
    [JsonPropertyName("wind")]
    public Wind Wind { get; set; } = new();
    
    [JsonPropertyName("clouds")]
    public Clouds Clouds { get; set; } = new();
    
    [JsonPropertyName("dt")]
    public long Dt { get; set; }
    
    [JsonPropertyName("sys")]
    public Sys Sys { get; set; } = new();
    
    [JsonPropertyName("timezone")]
    public int Timezone { get; set; }
    
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("cod")]
    public int Cod { get; set; }
}

public class Coordinates
{
    [JsonPropertyName("lon")]
    public double Lon { get; set; }
    
    [JsonPropertyName("lat")]
    public double Lat { get; set; }
}

public class Weather
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("main")]
    public string Main { get; set; } = string.Empty;
    
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
    
    [JsonPropertyName("icon")]
    public string Icon { get; set; } = string.Empty;
}

public class Main
{
    [JsonPropertyName("temp")]
    public double Temp { get; set; }
    
    [JsonPropertyName("feels_like")]
    public double FeelsLike { get; set; }
    
    [JsonPropertyName("temp_min")]
    public double TempMin { get; set; }
    
    [JsonPropertyName("temp_max")]
    public double TempMax { get; set; }
    
    [JsonPropertyName("pressure")]
    public int Pressure { get; set; }
    
    [JsonPropertyName("humidity")]
    public int Humidity { get; set; }
}

public class Wind
{
    [JsonPropertyName("speed")]
    public double Speed { get; set; }
    
    [JsonPropertyName("deg")]
    public int Deg { get; set; }
    
    [JsonPropertyName("gust")]
    public double? Gust { get; set; }
}

public class Clouds
{
    [JsonPropertyName("all")]
    public int All { get; set; }
}

public class Sys
{
    [JsonPropertyName("type")]
    public int Type { get; set; }
    
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("country")]
    public string Country { get; set; } = string.Empty;
    
    [JsonPropertyName("sunrise")]
    public long Sunrise { get; set; }
    
    [JsonPropertyName("sunset")]
    public long Sunset { get; set; }
}