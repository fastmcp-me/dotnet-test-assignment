using ModelContextProtocol.Client;

namespace WeatherMcpServer.Tests;

public class McpClientTests : TestBase
{
    [Fact]
    [Trait("Category", "Integration")]
    public async Task ListToolsAsync_ShouldReturnTools()
    {
        // Arrange
        await using var client = await McpClientFactory.CreateAsync(new StdioClientTransport(TransportOptions));

        // Act
        var toolsResult = await client.ListToolsAsync();

        // Assert
        Assert.NotNull(toolsResult);
        Assert.True(toolsResult.Any(), "Tools list should not be empty.");
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task CallToolAsync_GetCityWeather_ShouldReturnExpectedResult()
    {
        // Arrange
        await using var client = await McpClientFactory.CreateAsync(new StdioClientTransport(TransportOptions));
        var toolName = "get_city_weather";
        var arguments = new Dictionary<string, object?> { ["city"] = "Almaty" };

        // Act
        var callResult = await client.CallToolAsync(toolName, arguments);
        var textObj = callResult.Content.FirstOrDefault(c => c.Type == "text");

        // Assert
        Assert.NotNull(textObj);
        var textProp = textObj.GetType().GetProperty("Text");
        Assert.NotNull(textProp);
        var textValue = textProp.GetValue(textObj) as string;
        Assert.NotNull(textValue);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task CallToolAsync_GetWeatherForecast_ShouldReturnExpectedResult()
    {
        // Arrange
        await using var client = await McpClientFactory.CreateAsync(new StdioClientTransport(TransportOptions));
        var toolName = "get_weather_forecast";
        var arguments = new Dictionary<string, object?> { ["city"] = "Almaty", ["days"] = 4 };

        // Act
        var callResult = await client.CallToolAsync(toolName, arguments);
        var textObj = callResult.Content.FirstOrDefault(c => c.Type == "text");

        // Assert
        Assert.NotNull(textObj);
        var textProp = textObj.GetType().GetProperty("Text");
        Assert.NotNull(textProp);
        var textValue = textProp.GetValue(textObj) as string;
        Assert.NotNull(textValue);
    }
}
