namespace Identity.Api.Core;

/// <summary>
/// Marker interface for pipeline behaviours that decorate request handlers.
/// Behaviours are executed in the order they are registered in PipelineConfiguration.
/// </summary>
public interface IPipelineBehaviour
{
}

/// <summary>
/// Base interface for pipeline behaviours that wrap request handlers.
/// Behaviours are executed in the order they are configured.
/// </summary>
public interface IPipelineBehaviour<in TRequest, TResponse> : IPipelineBehaviour, IRequestHandler<TRequest, TResponse>
{
}
