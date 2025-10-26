using Identity.Api.Core;

namespace Identity.Api.Controllers.Refresh;

public record RefreshRequest(string RefreshToken) : IRequest<RefreshResponse>;