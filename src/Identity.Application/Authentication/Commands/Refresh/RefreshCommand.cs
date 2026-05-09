using Identity.Application.Common;

namespace Identity.Application.Authentication.Commands.Refresh;

public record RefreshCommand(string RefreshToken) : ICommand<RefreshResponse>;