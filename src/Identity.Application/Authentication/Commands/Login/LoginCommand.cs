using Identity.Application.Common;

namespace Identity.Application.Authentication.Commands.Login;

public record LoginCommand(string Email, string Password) : ICommand<LoginResponse>;