using Identity.Api.Core;

namespace Identity.Api.Behaviours;

/// <summary>
/// Pipeline behaviour that logs incoming requests.
/// Typically configured as the first (outermost) behaviour to capture all requests.
/// </summary>
public class LoggingBehaviour<TRequest, TResponse>(
    ILogger<LoggingBehaviour<TRequest, TResponse>> logger,
    IRequestHandler<TRequest, TResponse> inner) : IPipelineBehaviour<TRequest, TResponse>
{
    public async Task<TResponse> HandleAsync(TRequest request)
    {
        logger.LogInformation("Request: {Request}", request);

        return await inner.HandleAsync(request);
    }
}