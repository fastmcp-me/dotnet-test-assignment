# Weather MCP Server

This MCP server provides real-time weather information using the OpenWeatherMap API. It's built with the .NET MCP SDK and can be used with AI assistants like GitHub Copilot.

## Features

- **Current Weather**: Get the current weather conditions for any city.
- **Weather Forecast**: Get a 1-5-day weather forecast.
- **Weather Alerts**: Get weather alerts for any city.

## Project Structure

The project is organized as follows:

- **.mcp/**: Contains the MCP server manifest file (`server.json`).
- **Configurations/**: Contains configuration classes, such as `WeatherApiConfiguration`.
- **Exceptions/**: Contains custom exception types for the application.
- **Features/**: Implements the core logic using the CQRS pattern with MediatR.
  - **Behaviors/**: Contains MediatR pipeline behaviors for cross-cutting concerns like logging and validation.
  - **Queries/**: Contains the definitions of queries and their handlers.
  - **Services/**: Contains services that interact with external APIs, like `OpenWeatherMapService`.
- **Tools/**: Defines the tools exposed by the MCP server, such as `WeatherTools`.
- **Program.cs**: The main entry point of the application, where services are configured and the host is built.
- **appsettings.json**: The configuration file for the application.

## Configuration

To use this server, you need an API key from [OpenWeatherMap](https://openweathermap.org/api).

### appsettings.json

You can configure the API key in the `appsettings.json` file:
```json
{
  "WeatherApiConfiguration": {
    "ApiKey": "YOUR_API_KEY",
    "Url": "https://api.openweathermap.org/data/2.5/",
    "Units": "metric"
  }
}
```
Replace `YOUR_API_KEY` with your actual OpenWeatherMap API key.

### Environment Variables

Alternatively, you can configure the server using environment variables. The server will look for the following variables:

- `WeatherApiConfiguration__ApiKey`
- `WeatherApiConfiguration__Url`
- `WeatherApiConfiguration__Units`

## Developing Locally

To test this MCP server from source code, you can configure your IDE to run the project directly.

In your `.mcp.json` file, add the following server configuration:
{
  "servers": {
    "WeatherMcpServer": {
      "type": "stdio",
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "<PATH TO WeatherMcpServer.csproj>"
      ]
    }
  }
}
Replace `<PATH TO WeatherMcpServer.csproj>` with the absolute or relative path to the project file.

## Usage

Once configured, you can ask Copilot Chat for the weather. Here are some examples:

- `What's the weather in London?`
- `Get the weather forecast for New York for the next 3 days.`
- `Are there any weather alerts for Miami?`

## Using the MCP Server from NuGet.org

Once the MCP server package is published to NuGet.org, you can configure it in your preferred IDE.

- **VS Code**: Create a `<WORKSPACE DIRECTORY>/.vscode/mcp.json` file.
- **Visual Studio**: Create a `<SOLUTION DIRECTORY>/.mcp.json` file.

The configuration file uses the following server definition:
```json
{
  "servers": {
    "WeatherMcpServer": {
      "type": "stdio",
      "command": "dnx",
      "args": [
        "<your package ID here>",
        "--version",
        "<your package version here>",
        "--yes"
      ]
    }
  }
}
```
## More information

.NET MCP servers use the [ModelContextProtocol](https://www.nuget.org/packages/ModelContextProtocol) C# SDK. For more information about MCP:

- [Official Documentation](https://modelcontextprotocol.io/)
- [Protocol Specification](https://spec.modelcontextprotocol.io/)
- [GitHub Organization](https://github.com/modelcontextprotocol)
- 
Refer to the VS Code or Visual Studio documentation for more information on configuring and using MCP servers:

- [Use MCP servers in VS Code (Preview)](https://code.visualstudio.com/docs/copilot/chat/mcp-servers)
- [Use MCP servers in Visual Studio (Preview)](https://learn.microsoft.com/visualstudio/ide/mcp-servers)