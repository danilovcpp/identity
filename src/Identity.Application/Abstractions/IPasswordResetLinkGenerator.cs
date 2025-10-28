namespace Identity.Api.Abstractions;

public interface IPasswordResetLinkGenerator
{
    string CreatePasswordResetLink(string email, string token);
}
