using MediatR;
using Weather.Core.Interfaces;

namespace Weather.UseCases;

public class UseCaseDispatcher(IMediator mediator) : IUseCaseDispatcher
{
	public Task<TResponse> Send<TResponse>(IUseCaseRequest<TResponse> request, CancellationToken cancellationToken = default)
	{
		if (request is IRequest<TResponse> mediatrRequest)
        {
            return mediator.Send(mediatrRequest, cancellationToken);
        }

        throw new InvalidOperationException("Request is not a valid MediatR request.");
	}
}
