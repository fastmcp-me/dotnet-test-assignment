using Application.Common.AutoMapperProfiles;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Application.DependencyInjections;

public static class AutoMapServiceRegistrations
{
    public static IServiceCollection AddAutoMapServices(this IServiceCollection services,
        IConfiguration configuration, IHostEnvironment environment)
    {
        services.AddSingleton(new MapperConfiguration(config =>
        {
            #region Weathers
            config.AddProfile<WeatherMappingProfile>();

            #endregion




        }).CreateMapper());
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        return services;
    }
}