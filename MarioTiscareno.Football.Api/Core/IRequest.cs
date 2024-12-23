namespace MarioTiscareno.Football.Api.Core;

public interface IRequest<TResponse> { }

public interface IRequestHandler<TRequest, TResponse>
{
    Task<ResultOf<TResponse>> HandleAsync(
        TRequest request,
        CancellationToken cancellationToken = default
    );
}
