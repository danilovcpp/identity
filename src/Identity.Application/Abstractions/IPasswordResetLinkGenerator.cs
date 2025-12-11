namespace Identity.Application.Abstractions;

public interface IPasswordResetLinkGenerator
{
    string CreatePasswordResetLink(string email, string token);
}
