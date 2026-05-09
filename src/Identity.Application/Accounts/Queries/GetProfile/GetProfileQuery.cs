using Identity.Application.Common;

namespace Identity.Application.Accounts.Queries.GetProfile;

public record GetProfileQuery(string UserId) : IQuery<GetProfileResponse>;
