using Microsoft.Extensions.DependencyInjection;

namespace WeatherMcpServer.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IWeatherService, WeatherService>();

        return services;
    }
}
