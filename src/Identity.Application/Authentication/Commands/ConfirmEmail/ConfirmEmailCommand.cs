using Identity.Application.Common;

namespace Identity.Application.Authentication.Commands.ConfirmEmail;

public record ConfirmEmailCommand(string UserId, string Token) : ICommand<ConfirmEmailResponse>;