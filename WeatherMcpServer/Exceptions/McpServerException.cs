namespace WeatherMcpServer.Exceptions;

/// <summary>
/// Represents a general server exception.
/// </summary>
public class McpServerException(string message) : Exception(message);
