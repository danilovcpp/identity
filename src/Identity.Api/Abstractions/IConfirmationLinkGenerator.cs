namespace Identity.Api.Abstractions;

public interface IConfirmationLinkGenerator
{
    string CreateConfirmationLink(string userId, string token);
}