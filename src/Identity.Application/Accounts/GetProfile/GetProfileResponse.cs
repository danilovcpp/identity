namespace Identity.Application.GetProfile;

public record GetProfileResponse(
    string Id,
    string Email,
    string? UserName,
    string? AvatarUrl,
    bool EmailConfirmed,
    DateTimeOffset CreatedAt);
