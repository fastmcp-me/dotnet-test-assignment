using WeatherMcpServer.Features.Queries;
using WeatherMcpServer.Features.Services.DTOs.Interfaces;
using WeatherMcpServer.Features.Services.Interfaces;

namespace WeatherMcpServer.Tests.Queries;

public class GetWeatherAlertsTests
{
    private readonly Mock<IWeatherService> _weatherServiceMock;
    private readonly GetWeatherAlerts.Handler _handler;

    public GetWeatherAlertsTests()
    {
        _weatherServiceMock = new Mock<IWeatherService>();
        _handler = new GetWeatherAlerts.Handler(_weatherServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidQueryWithAlerts_ReturnsFormattedAlertString()
    {
        // Arrange
        var query = new GetWeatherAlerts.Query("Miami", "US");
        var alertData = CreateMockAlertData();

        _weatherServiceMock
            .Setup(x => x.GetWeatherAlerts("Miami", "US", It.IsAny<CancellationToken>()))
            .ReturnsAsync(alertData.Object);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().Contain("🚨 Weather alert for Miami:");
        result.Should().Contain("⚠️ Type: Hurricane");
        result.Should().Contain("📊 Severity: Severe");
        result.Should().Contain("🔍 Certainty: Likely");
        result.Should().Contain("📝 Description: Hurricane warning in effect");

        _weatherServiceMock.Verify(
            x => x.GetWeatherAlerts("Miami", "US", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_NoAlerts_ReturnsNoAlertsMessage()
    {
        // Arrange
        var query = new GetWeatherAlerts.Query("London", "UK");

        _weatherServiceMock
            .Setup(x => x.GetWeatherAlerts("London", "UK", It.IsAny<CancellationToken>()))
            .ReturnsAsync((IWeatherAlertDto?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().Be("No weather alerts for London.");

        _weatherServiceMock.Verify(
            x => x.GetWeatherAlerts("London", "UK", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_QueryWithoutCountryCode_CallsServiceCorrectly()
    {
        // Arrange
        var query = new GetWeatherAlerts.Query("Tokyo", null);
        var alertData = CreateMockAlertData("Earthquake", "Moderate", "Possible", "Earthquake alert");

        _weatherServiceMock
            .Setup(x => x.GetWeatherAlerts("Tokyo", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(alertData.Object);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().Contain("🚨 Weather alert for Tokyo:");
        result.Should().Contain("⚠️ Type: Earthquake");
        result.Should().Contain("📊 Severity: Moderate");
        result.Should().Contain("🔍 Certainty: Possible");
        result.Should().Contain("📝 Description: Earthquake alert");

        _weatherServiceMock.Verify(
            x => x.GetWeatherAlerts("Tokyo", null, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData("Flood", "Minor", "Observed", "Flood warning")]
    [InlineData("Tornado", "Extreme", "Likely", "Tornado alert")]
    [InlineData("Thunderstorm", "Moderate", "Possible", "Thunderstorm watch")]
    public async Task Handle_DifferentAlertTypes_FormatsCorrectly(
        string type, string severity, string certainty, string description)
    {
        // Arrange
        var query = new GetWeatherAlerts.Query("TestCity", "TC");
        var alertData = CreateMockAlertData(type, severity, certainty, description);

        _weatherServiceMock
            .Setup(x => x.GetWeatherAlerts(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(alertData.Object);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().Contain($"⚠️ Type: {type}");
        result.Should().Contain($"📊 Severity: {severity}");
        result.Should().Contain($"🔍 Certainty: {certainty}");
        result.Should().Contain($"📝 Description: {description}");
    }

    [Fact]
    public async Task Handle_ServiceThrowsException_PropagatesException()
    {
        // Arrange
        var query = new GetWeatherAlerts.Query("ErrorCity", null);
        _weatherServiceMock
            .Setup(x => x.GetWeatherAlerts(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Service error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_CancellationRequested_PropagatesCancellation()
    {
        // Arrange
        var query = new GetWeatherAlerts.Query("CancelCity", "CC");
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        _weatherServiceMock
            .Setup(x => x.GetWeatherAlerts(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _handler.Handle(query, cancellationTokenSource.Token));
    }

    private static Mock<IWeatherAlertDto> CreateMockAlertData(
        string type = "Hurricane",
        string severity = "Severe",
        string certainty = "Likely",
        string description = "Hurricane warning in effect")
    {
        var mock = new Mock<IWeatherAlertDto>();
        mock.Setup(x => x.Type).Returns(type);
        mock.Setup(x => x.Severity).Returns(severity);
        mock.Setup(x => x.Certainty).Returns(certainty);
        mock.Setup(x => x.Description).Returns(description);
        return mock;
    }
}