using Identity.Application.Common;

namespace Identity.Application.Authentication.Commands.ChangePassword;

public sealed record ChangePasswordCommand(
    string CurrentPassword,
    string NewPassword,
    string ConfirmNewPassword) : ICommand<ChangePasswordResponse>;
