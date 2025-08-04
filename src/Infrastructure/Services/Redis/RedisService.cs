using Domain.Common.Redis;
using Domain.Enums;
using StackExchange.Redis;
using System.Text.Json;

namespace Infrastructure.Services.Redis;

public class RedisService : IRedisService, IDisposable
{
    #region DI
    private IDatabase _dbRedis => _connection.GetDatabase((int)RedisDb.Weather);
    private readonly IConnectionMultiplexer _connection;

    public RedisService(IConnectionMultiplexer connection)
    {
        _connection = connection;
    }
    #endregion


    public async Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var json = JsonSerializer.Serialize(value);

        return await _dbRedis.StringSetAsync(key, json, expiry);
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var cashValue = await _dbRedis.StringGetAsync(key);
        if (cashValue.IsNullOrEmpty)
            return default(T);

        return JsonSerializer.Deserialize<T>(cashValue);        //cashValue.HasValue ? cashValue.ToString() : null;
    }

    public async Task<bool> KeyDeleteAsync(string key)
    {
        return await _dbRedis.KeyDeleteAsync(key);
    }

    public void Dispose()
    {
        _connection?.Dispose();
    }
}