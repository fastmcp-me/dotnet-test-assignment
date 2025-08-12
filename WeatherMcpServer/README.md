# Real Weather MCP Server

A comprehensive MCP (Model Context Protocol) server that provides real-time weather data through AI assistants like Claude. This server integrates with the OpenWeatherMap API to deliver accurate current weather conditions, forecasts, and weather alerts.

## Features

- **🌤️ Current Weather**: Get real-time weather conditions for any city worldwide
- **📅 Weather Forecast**: Retrieve up to 5-day weather forecasts with detailed information
- **⚠️ Weather Alerts**: Access weather warnings and alerts for specific locations (bonus feature)
- **🌍 Global Coverage**: Support for cities worldwide with country code specification
- **🔧 Robust Error Handling**: Comprehensive error handling and logging
- **📊 Rich Formatting**: Beautiful, formatted weather reports with emojis and structured data

## Prerequisites

- .NET 8.0 or later
- OpenWeatherMap API key (free tier available)

## Setup Instructions

### 1. Get OpenWeatherMap API Key

1. Visit [OpenWeatherMap](https://openweathermap.org/api)
2. Sign up for a free account
3. Navigate to API Keys section
4. Copy your API key

### 2. Configure Environment Variable

Set the `OPENWEATHER_API_KEY` environment variable:

**Windows (PowerShell):**
```powershell
$env:OPENWEATHER_API_KEY="your_api_key_here"
```

**Windows (Command Prompt):**
```cmd
set OPENWEATHER_API_KEY=your_api_key_here
```

**Linux/macOS:**
```bash
export OPENWEATHER_API_KEY="your_api_key_here"
```

### 3. Build and Run

```bash
dotnet build
dotnet run
```

## Available Tools

### GetCurrentWeather
Gets current weather conditions for a specified city.

**Parameters:**
- `city` (required): The city name to get weather for
- `countryCode` (optional): Country code (e.g., 'US', 'UK', 'RU')

**Example:** "What's the current weather in Moscow, RU?"

### GetWeatherForecast
Gets weather forecast for a specified city (up to 5 days).

**Parameters:**
- `city` (required): The city name to get weather forecast for
- `countryCode` (optional): Country code (e.g., 'US', 'UK', 'RU')
- `days` (optional): Number of days for forecast (1-5, default: 3)

**Example:** "Give me a 5-day weather forecast for London, UK"

### GetWeatherAlerts
Gets weather alerts and warnings for a specified city.

**Parameters:**
- `city` (required): The city name to get weather alerts for
- `countryCode` (optional): Country code (e.g., 'US', 'UK', 'RU')

**Example:** "Are there any weather alerts for Miami, US?"

## Developing Locally

To test this MCP server from source code locally, configure your IDE to run the project directly using `dotnet run`.

**VS Code Configuration (`.vscode/mcp.json`):**
```json
{
  "servers": {
    "WeatherMcpServer": {
      "type": "stdio",
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "path/to/WeatherMcpServer"
      ],
      "env": {
        "OPENWEATHER_API_KEY": "your_api_key_here"
      }
    }
  }
}
```

## Testing the MCP Server

Once configured, you can test the weather tools by asking questions like:
- "What's the current weather in Tokyo?"
- "Give me a 3-day forecast for Paris, FR"
- "Are there any weather alerts for New York, US?"

## Publishing to NuGet.org

1. Run `dotnet pack -c Release` to create the NuGet package
2. Publish to NuGet.org with `dotnet nuget push bin/Release/*.nupkg --api-key <your-api-key> --source https://api.nuget.org/v3/index.json`

## Using the MCP Server from NuGet.org

Once the MCP server package is published to NuGet.org, you can configure it in your preferred IDE. Both VS Code and Visual Studio use the `dnx` command to download and install the MCP server package from NuGet.org.

- **VS Code**: Create a `<WORKSPACE DIRECTORY>/.vscode/mcp.json` file
- **Visual Studio**: Create a `<SOLUTION DIRECTORY>\.mcp.json` file

For both VS Code and Visual Studio, the configuration file uses the following server definition:

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

Refer to the VS Code or Visual Studio documentation for more information on configuring and using MCP servers:

- [Use MCP servers in VS Code (Preview)](https://code.visualstudio.com/docs/copilot/chat/mcp-servers)
- [Use MCP servers in Visual Studio (Preview)](https://learn.microsoft.com/visualstudio/ide/mcp-servers)
