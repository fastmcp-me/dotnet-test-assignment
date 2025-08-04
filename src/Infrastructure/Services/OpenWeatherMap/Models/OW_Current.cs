namespace Infrastructure.Services.OpenWeatherMap.Models;

public class OW_Current
{
    public long Dt { get; set; }
    public OW_Main Main { get; set; } = default!;
    public List<OW_Weather> Weather { get; set; } = default!;
}