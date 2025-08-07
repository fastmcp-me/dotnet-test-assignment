using FluentValidation;
using MediatR;
using WeatherMcpServer.Exceptions;

namespace WeatherMcpServer.Features.Behaviors;

/// <summary>
/// A pipeline behavior that validates the request.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : class, IRequest<TResponse>
{
    /// <summary>
    /// Handles the request and performs validation.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="next">The next handler in the pipeline.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response.</returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);
        var errorsDictionary = (await Task.WhenAll(validators.Select(x => x.ValidateAsync(context))))
            .SelectMany(x => x.Errors)
            .Where(x => x != null)
            .GroupBy(
                x => x.PropertyName,
                x => x.ErrorMessage,
                (propertyName, errorMessages) => new
                {
                    Key = propertyName,
                    Values = errorMessages.Distinct().ToArray(),
                })
            .ToDictionary(x => x.Key, x => x.Values);

        if (errorsDictionary.Count != 0)
        {
            throw new ValidationCoreException(errorsDictionary);
        }

        return await next();

    }
}
