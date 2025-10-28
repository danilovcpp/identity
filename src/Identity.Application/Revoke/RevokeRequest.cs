using MediatR;

namespace Identity.Application.Revoke;

public record RevokeRequest(string RefreshToken) : IRequest<RevokeResponse>;