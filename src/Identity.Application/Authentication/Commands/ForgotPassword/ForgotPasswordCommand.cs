using Identity.Application.Common;

namespace Identity.Application.Authentication.Commands.ForgotPassword;

public record ForgotPasswordCommand(string Email) : ICommand<ForgotPasswordResponse>;
