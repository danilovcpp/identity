using MediatR;

namespace Identity.Application.Register;

public record RegisterRequest(string Email, string Password) : IRequest<RegisterResponse>;