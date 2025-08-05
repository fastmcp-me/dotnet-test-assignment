# WeatherMcpServer

A Model Context Protocol (MCP) server for retrieving weather information. This server provides tools that enable AI models to access current weather conditions, forecasts, and weather alerts for specified locations.

## Features

- Get current weather conditions for any city
- Retrieve weather forecasts for the next 8 days
- Access weather alerts for a specific location

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- An OpenWeatherMap API key
  - [Sign up](https://home.openweathermap.org/users/sign_up) for an OpenWeather account if you don't have one
  - After registration, get your API key from your [account page](https://home.openweathermap.org/api_keys)
  - The free tier includes 1,000 API calls per day at no cost

## Setup and Configuration

### Installation

1. Clone the repository:
   ```bash
   git clone <repository-url>
   cd WeatherMcpServer
   ```

2. Build the project:
   ```bash
   dotnet build
   ```

3. Configure OpenWeatherMap API:
   - Option 1: Using appsettings.json
     - Open `appsettings.json` 
     - Replace the `ApiKey` value with your own OpenWeatherMap API key:
     ```json
     "OpenWeatherMap": {
       "ApiKey": "your-api-key-here",
       "BaseUrl": "https://api.openweathermap.org"
     }
     ```
   - Option 2: Using environment variables
     - Set the following environment variable:
     ```bash
     # For macOS/Linux
     export OpenWeatherMap__ApiKey="your-api-key-here"
     
     # For Windows Command Prompt
     set OpenWeatherMap__ApiKey=your-api-key-here
     
     # For Windows PowerShell
     $env:OpenWeatherMap__ApiKey = "your-api-key-here"
     ```

## Running the Server

Run the server using the .NET CLI:

```bash
dotnet run
```

The server uses standard input/output for communication, following the MCP protocol specification.

## Usage Examples

The server exposes several MCP tools that can be called by MCP clients:


## Testing the MCP Server

Here are examples of testing each MCP tool with real responses:

### Testing Current Weather Tool

1. Configure your MCP client and server as described above
2. Ask: "What's the current weather in Astana?"
3. The system will use the `get_current_weather` tool and return something like:

```
Current Weather in Astana:

Condition: Clouds - overcast clouds
Temperature: 22.8°C
Feels Like: 22.3°C
Humidity: 46%
Wind Speed: 6.2 m/s
UV Index: 3.1
Cloud Cover: 100%
Visibility: 10.0 km
Sun Times: Sunrise at 06:02, Sunset at 18:09
```

### Testing Weather Forecast Tool

1. Ask: "What's the weather forecast for Astana for the next week?"
2. The system will use the `get_forecast` tool and return something like:

```
8-Day Weather Forecast for Astana
Thursday, July 31, 2025

Weather: Overcast clouds
Temperature: 22.43°C - 23.14°C
Day: 22.84°C, Night: 22.88°C
Cloud Cover: 100%
Summary: Partly cloudy conditions throughout the day

Friday, August 1, 2025
Weather: Overcast clouds
Temperature: 22.21°C - 22.79°C
Day: 22.57°C, Night: 22.30°C
Cloud Cover: 92%
Summary: Partly cloudy conditions throughout the day

Saturday, August 2, 2025
Weather: Overcast clouds
Temperature: 21.74°C - 22.55°C
Day: 22.09°C, Night: 22.05°C
Cloud Cover: 100%
Summary: Partly cloudy conditions throughout the day
[Additional days...]
```

### Testing Weather Alerts Tool

1. Ask: "Are there any weather alerts for Astana?"
2. The system will use the `get_alerts` tool and return something like:

```
Weather Alerts for Miami:

Weather Alerts for Astana:

🚨 Alert #1: HEAT ADVISORY
   Sender: National Weather Service
   Start: Jul 31, 2025 09:00
   End: Aug 01, 2025 21:00
   Description: Thunderstorms
   Tags: Thunderstorm

```

When testing, you may see different results based on current weather conditions, as the service retrieves real-time data from OpenWeatherMap.

## Implementation Approach

### Architecture

The server is built using the MCP (Model Context Protocol) framework with a practical layered architecture:

1. **Entry Point**: `Program.cs` - Sets up dependency injection, configuration, and initializes the MCP server with stdio transport
2. **Tools Layer**: Provides MCP-compatible tools that can be invoked by clients
3. **Services Layer**: Contains the business logic for retrieving and processing weather data
4. **DTOs**: Data Transfer Objects for exchanging data between layers

The architecture is designed to be straightforward and focused on the specific task of providing weather information via the MCP protocol. It maintains a separation of concerns while remaining lightweight and pragmatic.

### Key Components

- **WeatherTools**: Implements the MCP server tools for weather-related functionality
- **WeatherService**: Handles communication with the OpenWeatherMap API
- **WeatherFormatter**: Formats weather data into human-readable responses


## Publishing to NuGet.org

1. Run `dotnet pack -c Release` to create the NuGet package
2. Publish to NuGet.org with `dotnet nuget push bin/Release/*.nupkg --api-key <your-api-key> --source https://api.nuget.org/v3/index.json`

## Configuring in VS Code or Visual Studio

Once the MCP server package is published to NuGet.org, you can configure it in your preferred IDE:

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
        "SampleMcpServer",
        "--version",
        "0.1.0-beta",
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
