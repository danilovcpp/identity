using Identity.Application.Common;

namespace Identity.Application.Authentication.Commands.Register;

public sealed record RegisterCommand(
    string AccountName,
    string FirstName,
    string LastName,
    string Password) : ICommand<RegisterResponse>;