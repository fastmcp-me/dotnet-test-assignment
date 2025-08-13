namespace WeatherMcpServer.Extensions;

public static class DateTimeExtension
{
    public static DateTime ToDateTime(this long unixTimeSeconds) => 
        DateTimeOffset.FromUnixTimeSeconds(unixTimeSeconds).UtcDateTime;
}
