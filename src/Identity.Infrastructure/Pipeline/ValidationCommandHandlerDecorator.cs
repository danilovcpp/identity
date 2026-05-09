using FluentValidation;
using Identity.Application.Common;
using Identity.Core;

namespace Identity.Infrastructure.Pipeline;

public sealed class ValidationCommandHandlerDecorator<TCommand, TResponse>(
    ICommandHandler<TCommand, TResponse> inner,
    IEnumerable<IValidator<TCommand>> validators)
    : ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    public async Task<Result<TResponse>> HandleAsync(TCommand command, CancellationToken ct)
    {
        foreach (var v in validators)
        {
            var r = await v.ValidateAsync(command, ct);
            if (!r.IsValid)
            {
                var first = r.Errors[0];
                return new DomainError("validation." + first.PropertyName, first.ErrorMessage);
            }
        }
        return await inner.HandleAsync(command, ct);
    }
}