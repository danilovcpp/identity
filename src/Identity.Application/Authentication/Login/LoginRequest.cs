using Identity.Api.Controllers.Login;
using MediatR;

namespace Identity.Application.Login;

public record LoginRequest(string Email, string Password) : IRequest<LoginResponse>;