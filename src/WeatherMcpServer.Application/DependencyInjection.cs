using Microsoft.Extensions.DependencyInjection;
using WeatherMcpServer.Application.Abstractions;

namespace WeatherMcpServer.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IWeatherService, WeatherService>();

        return services;
    }
}
