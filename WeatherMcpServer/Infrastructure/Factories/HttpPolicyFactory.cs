using Polly.Extensions.Http;
using Polly;
using System.Net;
using Microsoft.Extensions.Configuration;
using WeatherMcpServer.Application.Interfaces;

namespace WeatherMcpServer.Infrastructure.Factories;

public class HttpPolicyFactory : IHttpPolicyFactory
{
    private readonly IConfiguration _config;

    public HttpPolicyFactory(IConfiguration config)
    {
        _config = config;
    }

    public IAsyncPolicy<HttpResponseMessage> CreateRetryPolicy()
    {
        var retryCount = _config.GetValue<int>("HttpClientPolicies:RetryCount", 3);
        var baseDelay = _config.GetValue<int>("HttpClientPolicies:RetryBaseDelaySeconds", 2);

        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(baseDelay, retryAttempt)));
    }

    public IAsyncPolicy<HttpResponseMessage> CreateCircuitBreakerPolicy()
    {
        var errorsAllowed = _config.GetValue<int>("HttpClientPolicies:CircuitBreakerAllowedErrors", 5);
        var duration = _config.GetValue<int>("HttpClientPolicies:CircuitBreakerDurationSeconds", 30);

        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(errorsAllowed, TimeSpan.FromSeconds(duration));
    }

    public IAsyncPolicy<HttpResponseMessage> CreateCombinedPolicy() =>
        Policy.WrapAsync(CreateRetryPolicy(), CreateCircuitBreakerPolicy());
}
