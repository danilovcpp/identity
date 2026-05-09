using Identity.Application.Common;

namespace Identity.Application.Authentication.Commands.ResetPassword;

public record ResetPasswordCommand(
    string Email,
    string Token,
    string NewPassword) : ICommand<ResetPasswordResponse>;
