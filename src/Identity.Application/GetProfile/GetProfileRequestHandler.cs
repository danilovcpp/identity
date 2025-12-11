using Identity.Application.GetProfile.Exceptions;
using Identity.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Identity.Application.GetProfile;

public class GetProfileRequestHandler(
    UserManager<ApplicationUser> userManager) : IRequestHandler<GetProfileRequest, GetProfileResponse>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<GetProfileResponse> Handle(GetProfileRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId) ?? throw new UserNotFoundException(request.UserId);

        return new GetProfileResponse(
            Id: user.Id,
            Email: user.Email!,
            UserName: user.UserName,
            AvatarUrl: user.AvatarUrl,
            EmailConfirmed: user.EmailConfirmed,
            CreatedAt: DateTimeOffset.UtcNow); // todo:
    }
}
