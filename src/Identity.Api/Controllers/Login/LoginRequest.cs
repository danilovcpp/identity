using Identity.Api.Core;

namespace Identity.Api.Controllers.Login;

public record LoginRequest(string Email, string Password) : IRequest<LoginResponse>;