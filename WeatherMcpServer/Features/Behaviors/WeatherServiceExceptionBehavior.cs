using MediatR;
using Microsoft.Extensions.Logging;
using WeatherMcpServer.Exceptions;
using WeatherMcpServer.Features.Queries;

namespace WeatherMcpServer.Features.Behaviors;

/// <summary>
/// A pipeline behavior that handles exceptions for weather service requests.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
internal class WeatherServiceExceptionBehavior<TRequest, TResponse>(
    ILogger<WeatherServiceExceptionBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse> 
        where TRequest : WeatherBase.Query, IRequest<TResponse> 
        where TResponse : class
{
    /// <summary>
    /// Handles the request and catches exceptions.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="next">The next handler in the pipeline.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response.</returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (ValidationCoreException ex)
        {
            logger.LogError(ex, "Request {RequestName} validation failed", typeof(TRequest).Name);

            var errors = ex.Errors
                .Select(x => $"Errors for property {x.Key}: {string.Join("\n", x.Value.Select((msg, index) => $"\t{index + 1}. {msg};"))}\n");

            return (TResponse)(object)string.Join("\n", errors);
        }
        catch (McpServerException ex)
        {
            logger.LogError(ex, "An error occurred during the execution of the request {RequestName}.", typeof(TRequest).Name);

            return (TResponse)(object)ex.Message;
        }
        catch (TaskCanceledException ex)
        {
            logger.LogError(ex, "Request {RequestName} timed out.", typeof(TRequest).Name);

            return (TResponse)(object)"Request timed out, please try again later";
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred during the execution of the request {RequestName}.", typeof(TRequest).Name);

            return (TResponse)(object)"An unexpected error occurred during the execution of the request. Please try again later.";
        }
    }
}
