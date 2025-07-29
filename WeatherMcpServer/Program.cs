using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WeatherMcpServer.Business.Infrastructure.Behaviors;
using WeatherMcpServer.Business.Infrastructure.Formatters;
using WeatherMcpServer.Business.Interfaces;
using WeatherMcpServer.Integrations.OpenWeatherMap.Models;
using WeatherMcpServer.Integrations.OpenWeatherMap.Services;
using WeatherMcpServer.Tools;

var builder = Host.CreateApplicationBuilder(args);

// Configure all logs to go to stderr (stdout is used for the MCP protocol messages).
builder.Logging.AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Trace);

// Add configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();

// Configure options
builder.Services.Configure<OpenWeatherMapConfiguration>(
    builder.Configuration.GetSection(OpenWeatherMapConfiguration.SectionName));

// Register HttpClient
builder.Services.AddSingleton<HttpClient>();

// Register weather provider
builder.Services.AddScoped<IWeatherProvider, OpenWeatherMapService>();

// Register formatters
builder.Services.AddSingleton<IWeatherResponseFormatter, WeatherResponseFormatter>();

// Add MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<Program>();
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ExceptionHandlingBehavior<,>));
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
});

// Add FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Add the MCP services: the transport to use (stdio) and the tools to register.
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<WeatherTools>();

await builder.Build().RunAsync();