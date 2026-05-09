using Identity.Application.Abstractions;
using Identity.Application.Common;
using Identity.Core;

namespace Identity.Application.Authentication.Commands.ChangePassword;

public class ChangePasswordCommandHandler(
    ICurrentUserService currentUserService) : ICommandHandler<ChangePasswordCommand, ChangePasswordResponse>
{
    public async Task<Result<ChangePasswordResponse>> HandleAsync(
        ChangePasswordCommand command,
        CancellationToken cancellationToken)
    {
        // var userId = currentUserService.UserId
        //     ?? throw new UnauthorizedAccessException("Пользователь не аутентифицирован");
        //
        // var user = await userManager.FindByIdAsync(userId)
        //     ?? throw new UnauthorizedAccessException("Пользователь не найден");
        //
        // var result = await userManager.ChangePasswordAsync(
        //     user,
        //     command.CurrentPassword,
        //     command.NewPassword);
        //
        // if (!result.Succeeded)
        // {
        //     var errors = result.Errors.Select(e => e.Description).ToList();
        //
        //     if (errors.Any(e => e.Contains("Incorrect password", StringComparison.OrdinalIgnoreCase)))
        //     {
        //         throw new InvalidCurrentPasswordException();
        //     }
        //
        //     throw new PasswordChangeFailedException(errors);
        // }

        return new ChangePasswordResponse(true, "Пароль успешно изменен");
    }
}
