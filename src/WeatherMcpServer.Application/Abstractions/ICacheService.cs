namespace WeatherMcpServer.Application.Abstractions;

public interface ICacheService
{
    T? Get<T>(string key);
    void Set<T>(string key, T value, TimeSpan duration);
}
