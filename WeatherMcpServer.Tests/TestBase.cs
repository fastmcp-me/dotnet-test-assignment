using Microsoft.Extensions.Configuration;
using ModelContextProtocol.Client;

namespace WeatherMcpServer.Tests;

public abstract class TestBase
{
    protected IConfiguration Configuration { get; private set; }
    protected StdioClientTransportOptions TransportOptions { get; private set; }

    protected TestBase()
    {
        Configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("fakeappsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var serverProjectPath = GetProjectPath();

        var args = Configuration.GetSection("TransportOptions:Arguments").Get<List<string>>()!;
        args.Add(serverProjectPath);
        TransportOptions = new StdioClientTransportOptions
        {
            Name = Configuration["TransportOptions:Name"],
            Command = Configuration["TransportOptions:Command"]!,
            Arguments = args
        };
    }

    private static string GetProjectPath()
    {
        var assemblyLocation = Path.GetDirectoryName(typeof(AssemblyReference).Assembly.Location);
        if (assemblyLocation == null)
        {
            throw new InvalidOperationException("Unable to determine the assembly location.");
        }

        var projectPath = Path.Combine(assemblyLocation, "..", "..", "..", "..", "WeatherMcpServer", "WeatherMcpServer.csproj");
        return Path.GetFullPath(projectPath);
    }

}
