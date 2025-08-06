# Real Weather Mcp Server 

This is a .NET 10 MCP Server that provides **real-time weather data** using [WeatherAPI.com](https://www.weatherapi.com/) via the [Model Context Protocol (MCP)](https://modelcontextprotocol.io/) standard.

It was created as part of the FastMCP assignment and showcases full integration with the `.NET MCP Server` framework, weather tooling, structured responses, and rich test coverage.

## Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/ru-ru/download/dotnet/10.0)
- Free [openweathermap.org](https://openweathermap.org/) account

## Using the MCP Server from NuGet.org

Package name: [OKtol.WeatherMcpServer](https://www.nuget.org/packages/OKtol.WeatherMcpServer/)

1. Search for MCP server package on [NuGet.org](https://www.nuget.org/packages) and select [it](https://www.nuget.org/packages/OKtol.WeatherMcpServer/) from the list.

2. View the package details and copy the JSON from the "MCP Server" tab.

3. Create a mcp configuration file.
    - **VS Code**: `<WORKSPACE DIRECTORY>/.vscode/mcp.json`
    - **Visual Studio**: `<SOLUTION DIRECTORY>\.mcp.json`

    For both VS Code and Visual Studio, the configuration file uses the following server definition:

4. In your mcp.json file in the .vscode folder, add the copied JSON, which looks like this:
    ```json
    {
      "inputs": [
        {
          "type": "promptString",
          "id": "openweather_api_key",
          "description": "Api key from your https://api.openweathermap.org/ account.",
          "password": true
        }
      ],
      "servers": {
        "OKtol.WeatherMcpServer": {
          "type": "stdio",
          "command": "dnx",
          "args": ["OKtol.WeatherMcpServer@", "--yes"],
          "env": {
            "OpenWeatherOptions__BaseUrl": "https://api.openweathermap.org/",
            "OpenWeatherOptions__ApiKey": "${input:openweather_api_key}"
          }
        }
      }
    }
    ```

5. Save the file.

6. Click `Restart` button in this file.

7. In GitHub Copilot, select the Select tools icon to verify your `WeatherMcpServer` is available with the tools listed.

## Using the MCP Server locally

1.  Clone the repository:

    ```bash
    git clone <repository_url>
    ```

2.  Navigate to the project directory:

    ```bash
    cd dotnet-test-assignment/WeatherMcpServer
    ```

3.  Build the project:

    ```bash
    dotnet build
    ```

4. Configuration:

    The API key must be provided via an environment variable:

    ```bash
    OpenWeatherOptions__ApiKey=your_actual_key_here
    ```

    You can either:

    * Set it globally:
      `export OpenWeatherOptions__ApiKey=...` (Linux/macOS)
      `set OpenWeatherOptions__ApiKey=...` (Windows)

    * **Or use `appsettings.json` locally(recommended):**

    ```json
    {
      "OpenWeatherOptions": {
        "ApiKey": "your_actual_key_here"
      }
    }
    ```

5. You can run the server manually:

    ```bash
    dotnet run --project WeatherMcpServer
    ```

    Or test it with [MCP Inspector](https://github.com/modelcontextprotocol/inspector):

    ```bash
    npx @modelcontextprotocol/inspector
    ```

6. In GitHub Copilot, select the Select tools icon to verify your `WeatherMcpServer` is available with the tools listed.

## Testing the MCP Server

Once configured, you can ask Copilot Chat for a weather, for example, `What is the current weather in London?`. It should prompt you to use the `GetCurrentWeather` tool on the `WeatherMcpServer` MCP server and show you the results.

## More information

.NET MCP servers use the [ModelContextProtocol](https://www.nuget.org/packages/ModelContextProtocol) C# SDK. For more information about MCP:

- [Official Documentation](https://modelcontextprotocol.io/)
- [Protocol Specification](https://spec.modelcontextprotocol.io/)
- [GitHub Organization](https://github.com/modelcontextprotocol)

Refer to the VS Code or Visual Studio documentation for more information on configuring and using MCP servers:

- [Use MCP servers in VS Code (Preview)](https://code.visualstudio.com/docs/copilot/chat/mcp-servers)
- [Use MCP servers in Visual Studio (Preview)](https://learn.microsoft.com/visualstudio/ide/mcp-servers)

👨‍💻 Author
Built by OKtol <br>
Repo: https://github.com/OKtol/dotnet-test-assignment
