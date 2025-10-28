using System.Web;
using Identity.Api.Abstractions;

namespace Identity.Api.Services;

public class PasswordResetLinkGenerator : IPasswordResetLinkGenerator
{
    public string CreatePasswordResetLink(string email, string token)
    {
        var encodedToken = HttpUtility.UrlEncode(token);
        var encodedEmail = HttpUtility.UrlEncode(email);
        var scheme = ""; // todo: взять из Options
        var host = "";

        return $"{scheme}://{host}/api/reset-password?email={encodedEmail}&token={encodedToken}";
    }
}
