namespace Identity.Api.Core;

public interface IRequestHandler<in TRequest>
{
    Task HandleAsync(TRequest request);
}

public interface IRequestHandler<in TRequest, TResponse>
{
    Task<TResponse> HandleAsync(TRequest request);
}