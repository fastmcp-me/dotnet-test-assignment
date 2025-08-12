using System;

namespace Weather.Core.Interfaces;

public interface IUseCaseDispatcher
{
    Task<TResponse> Send<TResponse>(IUseCaseRequest<TResponse> request, CancellationToken cancellationToken = default);
}
