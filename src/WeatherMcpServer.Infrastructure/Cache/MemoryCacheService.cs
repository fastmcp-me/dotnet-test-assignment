using Microsoft.Extensions.Caching.Memory;
using WeatherMcpServer.Application.Abstractions;

namespace WeatherMcpServer.Infrastructure.Cache;

internal class MemoryCacheService
    : ICacheService
{
    private readonly IMemoryCache _cache;

    public MemoryCacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public T? Get<T>(string key)
    {
        return _cache.TryGetValue(key, out T? value) ? value : default;
    }

    public void Set<T>(string key, T value, TimeSpan duration)
    {
        _cache.Set(key, value, duration);
    }
}
