using System.ComponentModel;
using ModelContextProtocol.Server;

namespace Api.Tools;

public class GreetingTools
{
    [McpServerTool]
    [Description("Returns a friendly greeting")]
    public string SayHello(
        [Description("Name to greet")] string name = "World")
        => $"Hello, {name}! 👋";
}