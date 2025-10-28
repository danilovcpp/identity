using MediatR;

namespace Identity.Application.Refresh;

public record RefreshRequest(string RefreshToken) : IRequest<RefreshResponse>;