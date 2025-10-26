using Identity.Api.Core;

namespace Identity.Api.Controllers.Register;

public record RegisterRequest(string Email, string Password) : IRequest<RegisterResponse>;