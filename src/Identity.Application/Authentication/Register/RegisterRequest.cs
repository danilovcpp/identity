using Identity.Application.Register;
using MediatR;

namespace Identity.Application.Authentication.Register;

public sealed record RegisterRequest(string Email, string Password) : IRequest<RegisterResponse>;