using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WeatherMcpServer.Configurations;
using WeatherMcpServer.Features.Behaviors;
using WeatherMcpServer.Features.Services;
using WeatherMcpServer.Tools;
using System.Reflection;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Debug);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables("WeatherApiConfiguration");

builder.Services.Configure<WeatherApiConfiguration>(builder.Configuration.GetSection(nameof(WeatherApiConfiguration)));
builder.Services.AddWeatherService();
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
builder.Services.AddCoreBehaviors();

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<WeatherTools>();

await builder.Build().RunAsync();