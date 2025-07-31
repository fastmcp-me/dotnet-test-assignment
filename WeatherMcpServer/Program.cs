using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WeatherMcpServer.Options;
using WeatherMcpServer.Tools;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddOptions<OpenWeatherOptions>()
    .Bind(builder.Configuration.GetSection(nameof(OpenWeatherOptions)))
    .ValidateDataAnnotations()
    .ValidateOnStart();

// Configure all logs to go to stderr (stdout is used for the MCP protocol messages).
builder.Logging.AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Debug);

// Add the MCP services: the transport to use (stdio) and the tools to register.
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<RandomNumberTools>()
    .WithTools<WeatherTools>();

await builder.Build().RunAsync();