using Domain.Common.Redis;
using Domain.Common.Repositories;
using Infrastructure.Contexts;
using Infrastructure.Interceptors;
using Infrastructure.Services.Redis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

namespace Infrastructure.DependencyInjections;

public static class DalServiceRegistrations
{
    public static IServiceCollection AddDalServices(this IServiceCollection services,
            IConfiguration configuration, IHostEnvironment environment)
    {
        services.AddDbContext<dbContext>((context, options) =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("ConnectionStrDb"),
                builder =>
                {
                    builder.MigrationsAssembly("Infrastructure");
                    //builder.UseVector();
                }
            );
            options.AddInterceptors(context.GetRequiredService<AuditInterceptor>());
        });

        services.AddTransient<IdbContext, dbContext>();


        #region Redis
        string redisConnectionString = configuration.GetConnectionString("Redis")!;
        IConnectionMultiplexer multiplexer = ConnectionMultiplexer.Connect(redisConnectionString);
        services.AddSingleton<IConnectionMultiplexer>(multiplexer);

        services.AddSingleton<IRedisService, RedisService>();
        #endregion



        #region Interceptors
        services.AddScoped<AuditInterceptor>();

        #endregion

        return services;
    }
}