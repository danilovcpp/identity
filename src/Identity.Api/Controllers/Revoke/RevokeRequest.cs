using Identity.Api.Core;

namespace Identity.Api.Controllers.Revoke;

public record RevokeRequest(string RefreshToken) : IRequest<RevokeResponse>;