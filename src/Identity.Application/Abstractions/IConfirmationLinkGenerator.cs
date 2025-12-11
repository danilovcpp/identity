namespace Identity.Application.Abstractions;

public interface IConfirmationLinkGenerator
{
    string CreateConfirmationLink(string userId, string token);
}