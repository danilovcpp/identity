using Identity.Application.Abstractions;
using Identity.Application.Abstractions.Persistence;
using Identity.Application.Common;
using Identity.Core;
using Identity.Domain.Users;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Authentication.Commands.Register;

public sealed class RegisterCommandHandler(
    ILogger<RegisterCommandHandler> logger,
    IClock clock,
    IUserRepository userRepository,
    IEmailConfirmationService emailConfirmationService) : ICommandHandler<RegisterCommand, RegisterResponse>
{
    public async Task<Result<RegisterResponse>> HandleAsync(
        RegisterCommand command,
        CancellationToken cancellationToken)
    {
        var result = User.Create(
            command.AccountName,
            command.FirstName,
            command.LastName,
            clock);

        var user = result.Value;

        userRepository.Add(user);

        await emailConfirmationService.SendConfirmationLink(user);

        return new RegisterResponse();
    }
}