using Identity.Application.Abstractions;
using Identity.Application.ChangePassword.Exceptions;
using Identity.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Identity.Application.ChangePassword;

public class ChangePasswordRequestHandler(
    ICurrentUserService currentUserService,
    UserManager<ApplicationUser> userManager) : IRequestHandler<ChangePasswordRequest, ChangePasswordResponse>
{
    public async Task<ChangePasswordResponse> Handle(ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId
            ?? throw new UnauthorizedAccessException("Пользователь не аутентифицирован");

        var user = await userManager.FindByIdAsync(userId)
            ?? throw new UnauthorizedAccessException("Пользователь не найден");

        var result = await userManager.ChangePasswordAsync(
            user,
            request.CurrentPassword,
            request.NewPassword);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();

            if (errors.Any(e => e.Contains("Incorrect password", StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidCurrentPasswordException();
            }

            throw new PasswordChangeFailedException(errors);
        }

        return new ChangePasswordResponse(true, "Пароль успешно изменен");
    }
}
