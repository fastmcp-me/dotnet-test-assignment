namespace WeatherMcpServer.Application.Interfaces;

public interface IRateLimiter
{
    bool Allowed(string callerId);
}
