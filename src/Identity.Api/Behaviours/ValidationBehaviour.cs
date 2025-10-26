using Identity.Api.Core;

namespace Identity.Api.Behaviours;

/// <summary>
/// Pipeline behaviour that validates incoming requests.
/// Typically configured after logging but before business logic handlers.
/// </summary>
public class ValidationBehaviour<TRequest, TResponse>(
    ILogger<ValidationBehaviour<TRequest, TResponse>> logger,
    IRequestHandler<TRequest, TResponse> inner) : IPipelineBehaviour<TRequest, TResponse>
{
    public async Task<TResponse> HandleAsync(TRequest request)
    {
        // Example: You can add validation logic here
        // For now, just log that validation passed
        logger.LogDebug("Validating request of type {RequestType}", typeof(TRequest).Name);

        // Call next handler in pipeline
        return await inner.HandleAsync(request);
    }
}
