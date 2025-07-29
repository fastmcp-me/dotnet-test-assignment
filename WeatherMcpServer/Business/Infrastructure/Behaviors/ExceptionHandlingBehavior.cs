using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using WeatherMcpServer.Business.Infrastructure.Exceptions;

namespace WeatherMcpServer.Business.Infrastructure.Behaviors;

public class ExceptionHandlingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<ExceptionHandlingBehavior<TRequest, TResponse>> _logger;

    public ExceptionHandlingBehavior(ILogger<ExceptionHandlingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (ValidationException ex)
        {
            var requestName = typeof(TRequest).Name;
            _logger.LogWarning(ex, "Validation failed for request {RequestName}: {@Request}", requestName, request);
            
            var errors = string.Join("; ", ex.Errors.Select(e => e.ErrorMessage));
            throw new WeatherServiceException($"Invalid request: {errors}");
        }
        catch (WeatherServiceException)
        {
            // Re-throw business exceptions as they are already properly formatted
            throw;
        }
        catch (HttpRequestException ex)
        {
            var requestName = typeof(TRequest).Name;
            _logger.LogError(ex, "HTTP error occurred while processing {RequestName}: {@Request}", requestName, request);
            
            throw new WeatherServiceException(
                "Failed to connect to weather service. Please check your internet connection and try again.");
        }
        catch (TaskCanceledException ex)
        {
            var requestName = typeof(TRequest).Name;
            _logger.LogError(ex, "Request timeout while processing {RequestName}: {@Request}", requestName, request);
            
            throw new WeatherServiceException(
                "Request timed out. The weather service is taking too long to respond. Please try again.");
        }
        catch (Exception ex)
        {
            var requestName = typeof(TRequest).Name;
            _logger.LogError(ex, "Unexpected error occurred while processing {RequestName}: {@Request}", requestName, request);
            
            throw new WeatherServiceException(
                "An unexpected error occurred while processing your request. Please try again later.");
        }
    }
}