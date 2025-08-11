using Polly;

namespace WeatherMcpServer.Infrastructure.DelegatingHandlers;

public class PolicyHttpMessageHandler : DelegatingHandler
{
    private readonly IAsyncPolicy<HttpResponseMessage> _policy;

    public PolicyHttpMessageHandler(IAsyncPolicy<HttpResponseMessage> policy)
    {
        _policy = policy ?? throw new ArgumentNullException(nameof(policy));
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return _policy.ExecuteAsync(ct => base.SendAsync(request, ct), cancellationToken);
    }
}
