using System.Collections.Concurrent;
using WeatherMcpServer.Application.Interfaces;

namespace WeatherMcpServer.CrossCutting;

public class TokenBucketRateLimiter : IRateLimiter
{
    private record Bucket(double Tokens, DateTime LastRefill, double Capacity, double RefillRatePerSec);
    private readonly ConcurrentDictionary<string, (double tokens, DateTime lastRefill)> _buckets = new();
    private readonly double _capacity;
    private readonly double _refillPerSec;

    public TokenBucketRateLimiter(double capacity = 10, double refillPerSecond = 1)
    {
        _capacity = capacity;
        _refillPerSec = refillPerSecond;
    }

    public bool Allowed(string callerId)
    {
        var now = DateTime.UtcNow;
        var state = _buckets.GetOrAdd(callerId, (_capacity, now));
        var (tokens, last) = state;
        var delta = (now - last).TotalSeconds * _refillPerSec;
        tokens = Math.Min(_capacity, tokens + delta);
        if (tokens < 1)
        {
            _buckets[callerId] = (tokens, now);
            return false;
        }
        tokens -= 1;
        _buckets[callerId] = (tokens, now);
        return true;
    }
}
