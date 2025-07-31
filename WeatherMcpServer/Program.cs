using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WeatherMcpServer.Tools;
using WeatherMcpServer.Services;
using WeatherMcpServer.Commands;
using MediatR;
using Serilog;
using Serilog.Events;
using DotNetEnv;

Env.Load();

var builder = Host.CreateApplicationBuilder(args);

// Configure Serilog through DI
builder.Services.AddSerilog(configuration =>
{
    configuration
        .MinimumLevel.Debug()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .MinimumLevel.Override("System", LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.Extensions.Http", LogEventLevel.Information)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Application", "WeatherMcpServer")
        .Enrich.WithProperty("Version", "1.0.0")
        .WriteTo.Console(
            outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{Application}] {Message:lj} {Properties:j}{NewLine}{Exception}",
            restrictedToMinimumLevel: LogEventLevel.Information)
        .WriteTo.File("logs/weather-mcp-.txt", 
            rollingInterval: RollingInterval.Day,
            restrictedToMinimumLevel: LogEventLevel.Debug,
            outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] [{Application}] {SourceContext} {Message:lj} {Properties:j}{NewLine}{Exception}");
});

// Add HttpClient
builder.Services.AddHttpClient<IWeatherService, WeatherService>(client =>
{
    client.BaseAddress = new Uri("https://api.openweathermap.org/");
    client.DefaultRequestHeaders.Add("User-Agent", "WeatherMcpServer/1.0");
});

// Add MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// Add Weather Service
builder.Services.AddScoped<IWeatherService, WeatherService>();

// Add the MCP services: the transport to use (stdio) and the tools to register.
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<WeatherTools>();

var host = builder.Build();

var logger = Log.ForContext<Program>();

try
{
    logger.Information("Starting Weather MCP Server with PID {ProcessId}", Environment.ProcessId);
    logger.Information("Environment: {Environment}", builder.Environment.EnvironmentName);
    logger.Information("API Key configured: {ApiKeyConfigured}", !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("OPENWEATHERMAP_API_KEY")));
    
    await host.RunAsync();
}
catch (Exception ex)
{
    logger.Fatal(ex, "Weather MCP Server terminated unexpectedly");
    throw;
}
finally
{
    logger.Information("Weather MCP Server shutting down");
    await Log.CloseAndFlushAsync();
}