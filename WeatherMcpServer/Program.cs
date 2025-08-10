using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WeatherMcpServer.Application.Interfaces;
using WeatherMcpServer.CrossCutting;
using WeatherMcpServer.Domain;
using WeatherMcpServer.Infrastructure;
using WeatherMcpServer.Infrastructure.Factories;
using WeatherMcpServer.Infrastructure.Logging;
using WeatherMcpServer.Infrastructure.Providers;
using WeatherMcpServer.Tools;

var builder = Host.CreateApplicationBuilder(args);

// Configure all logs to go to stderr (stdout is used for the MCP protocol messages).
builder.Logging.AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Trace);
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

builder.Services.AddSingleton<IConfiguration>(configuration);

builder.Services.Configure<Dictionary<string, WeatherProviderConfig>>(
    builder.Configuration.GetSection("WeatherProviders"));

builder.Services.AddSingleton<IHttpPolicyFactory, HttpPolicyFactory>();
builder.Services.AddSingleton<ILinkBuilder, LinkBuilder>();
builder.Services.AddSingleton<ICustomHttpClientFactory, CustomHttpClientFactory>();
builder.Services.AddSingleton<IRateLimiter>(new TokenBucketRateLimiter(capacity: 20, refillPerSecond: 1));
builder.Services.AddSingleton<IProviderSelector, PriorityProviderSelector>();

builder.Services.AddHttpClient("OpenWeather")
    .SetHandlerLifetime(TimeSpan.FromMinutes(5))
    .AddPolicyHandler((sp, request) =>
        sp.GetRequiredService<IHttpPolicyFactory>().CreateRetryPolicy())
    .AddPolicyHandler((sp, request) =>
        sp.GetRequiredService<IHttpPolicyFactory>().CreateCircuitBreakerPolicy());

builder.Services.Scan(scan => scan
    .FromAssemblyOf<IWeatherMediator>()
    .AddClasses(classes => classes.AssignableTo<IWeatherMediator>())
        .AsImplementedInterfaces()
        .WithScopedLifetime()
    .AddClasses(classes => classes.AssignableTo<IWeatherProvider>())
        .AsImplementedInterfaces()
        .WithScopedLifetime()
);

builder.Services.Decorate<IWeatherMediator, LoggingWeatherMediatorDecorator>();
builder.Services.Decorate<IWeatherProvider, LoggingWeatherProviderDecorator>();

using var sp = builder.Services.BuildServiceProvider();

var orchestrator = sp.GetRequiredService<IWeatherMediator>();

Console.WriteLine("[DEBUG] Запрашиваем погоду...");
var result = await orchestrator.GetForecastAsync("Almaty", 4);
var temp = result.Value!.ToList();


// Add the MCP services: the transport to use (stdio) and the tools to register.
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<RandomNumberTools>()
    .WithTools<WeatherTools>();

await builder.Build().RunAsync();