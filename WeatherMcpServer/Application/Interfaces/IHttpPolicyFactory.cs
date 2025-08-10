using Polly;

namespace WeatherMcpServer.Application.Interfaces;

public interface IHttpPolicyFactory
{
    IAsyncPolicy<HttpResponseMessage> CreateRetryPolicy();
    IAsyncPolicy<HttpResponseMessage> CreateCircuitBreakerPolicy();
    IAsyncPolicy<HttpResponseMessage> CreateCombinedPolicy();
}
