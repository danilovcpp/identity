using MediatR;

namespace Identity.Application.GetProfile;

public record GetProfileRequest(string UserId) : IRequest<GetProfileResponse>;
