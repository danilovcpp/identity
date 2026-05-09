using Identity.Application.Common;
using Identity.Core;

namespace Identity.Application.Accounts.Queries.GetProfile;

public class GetProfileQueryHandler(
    ) : IQueryHandler<GetProfileQuery, GetProfileResponse>
{
    public async Task<Result<GetProfileResponse>> HandleAsync(
        GetProfileQuery query,
        CancellationToken cancellationToken)
    {
        //var user = await _userManager.FindByIdAsync(query.UserId) ?? throw new UserNotFoundException(query.UserId);

        // return new GetProfileResponse(
        //     Id: user.Id,
        //     Email: user.Email!,
        //     UserName: user.UserName,
        //     AvatarUrl: user.AvatarUrl,
        //     EmailConfirmed: user.EmailConfirmed,
        //     CreatedAt: DateTimeOffset.UtcNow); // todo:
        throw new NotImplementedException();
    }
}
