namespace Infrastructure.Services.OpenWeatherMap.Models.Alerts;

public class OW_Alert
{
    public string Sender_name { get; set; } = default!;
    public string Event { get; set; } = default!;
    public long Start { get; set; }
    public long End { get; set; }
    public string Description { get; set; } = default!;
}