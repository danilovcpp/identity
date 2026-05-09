using Identity.Application.Common;
using Identity.Core;
using Microsoft.Extensions.Logging;

namespace Identity.Infrastructure.Pipeline;

public sealed class LoggingCommandHandlerDecorator<TCommand, TResponse>(
    ILogger<TCommand> logger,
    ICommandHandler<TCommand, TResponse> inner)
    : ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    public async Task<Result<TResponse>> HandleAsync(TCommand command, CancellationToken ct)
    {
        logger.LogInformation("Command: {Command}", command);

        return await inner.HandleAsync(command, ct);
    }
}