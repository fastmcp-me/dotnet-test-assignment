using WeatherMcpServer.Application.Interfaces;
using WeatherMcpServer.Domain;

namespace WeatherMcpServer.Application;

public class WeatherMediator : IWeatherMediator
{
    private readonly IEnumerable<IWeatherProvider> _providers;
    private readonly IProviderSelector _selector;
    private readonly IRateLimiter _rateLimiter;
    private readonly IWeatherProvider _fallbackProvider;

    public WeatherMediator(
        IEnumerable<IWeatherProvider> providers,
        IProviderSelector selector,
        IRateLimiter rateLimiter,
        IWeatherProvider fallbackProvider)
    {
        _providers = providers;
        _selector = selector;
        _rateLimiter = rateLimiter;
        _fallbackProvider = fallbackProvider;
    }

    public Task<Result<WeatherResult>> GetCurrentWeatherAsync(string location, CancellationToken ct = default) =>
        Execute(location,
            p => p.GetCurrentWeatherAsync(location, ct),
            () => _fallbackProvider.GetCurrentWeatherAsync(location, ct));

    public Task<Result<IEnumerable<WeatherResult>>> GetForecastAsync(string location, int days = 3, CancellationToken ct = default) =>
        Execute(location,
            p => p.GetForecastAsync(location, days, ct),
            () => _fallbackProvider.GetForecastAsync(location, days, ct));

    private async Task<Result<T>> Execute<T>(
        string location,
        Func<IWeatherProvider, Task<T>> operation,
        Func<Task<T>> fallback)
    {
        if (!_rateLimiter.Allowed("global"))
            return Result<T>.Fail("Rate limit exceeded");

        foreach (var provider in _selector.Select(_providers))
        {
            try
            {
                var data = await operation(provider);
                if (data != null)
                    return Result<T>.Ok(data);
            }
            catch { }
        }

        try
        {
            var fallbackData = await fallback();
            if (fallbackData != null)
                return Result<T>.Ok(fallbackData);

            return Result<T>.Fail("No data from fallback provider");
        }
        catch (Exception ex)
        {
            return Result<T>.Fail("Fallback provider error", ex);
        }
    }
}
