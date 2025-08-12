# Description

1. I've implemented 3 MCPTools.
2. Two of them were tested using GitHub Copilot and xUnit
3. Alerts were not tested due to apiKey license restriction
4. To set up code and check functionality you may use VSCode with following code (create field mcp.json inside .vscode directory)
   
    ```json
    //.vscode/mcp.json
    {
        "servers": {
            "simple-mcp-server": {
                "type": "stdio",
                "command": "dotnet",
                "args": [
                    "run",
                    "--project",
                    "${workspaceFolder}/src/Weather.McpServer/Weather.McpServer.csproj",
                    "--configuration",
                    "Debug"
                ],
                "env": {
                    "DOTNET_ENVIRONMENT": "Development"
                },
                "cwd": "${workspaceFolder}/artifacts/bin/Weather.McpServer/debug",
            }
        }
    }
    ```
5. Project is divided in 3 logical parts: 
   1. Weather.McpServer - the MCP server itself
   2. Weather.Core - abstractions
   3. Weather.Infrastructure - WeatherAPI calls
   4. Weather.UseCases - use cases
6. Logs/Errors are managed by Serilog (I save them to file)
7. Central Package Management is set 
8. xUnit tests require **devproxy** to be set
9. In addition to STDOUT server it's possible to add http endposints and integrate everything into .NET Aspire, which will make code to be ready for production set up and open door for simple telemetry usage and setting.
10. MCP Tool servers are truly exciting thing to work with. Thank you!

## Example calls

1. Write to Copliot
   ```
   #simple-mcp-server get weather in London at the moment
    ```
2. Copliot answers
    ```
    The current weather in London is few clouds with a temperature of 26.55°C. Data was retrieved at 13:21:29 UTC.
    ```