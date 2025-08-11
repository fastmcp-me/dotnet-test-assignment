namespace WeatherMcpServer.Domain;

public class HttpClientPoliciesConfig
{
    public int RetryCount { get; set; }
    public int RetryBaseDelaySeconds { get; set; }
    public int CircuitBreakerAllowedErrors { get; set; }
    public int CircuitBreakerDurationSeconds { get; set; }
}
