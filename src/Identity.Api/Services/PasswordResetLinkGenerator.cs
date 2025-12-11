using System.Web;
using Identity.Application.Abstractions;

namespace Identity.Api.Services;

public class PasswordResetLinkGenerator : IPasswordResetLinkGenerator
{
    public string CreatePasswordResetLink(string email, string token)
    {
        var encodedToken = HttpUtility.UrlEncode(token);
        var encodedEmail = HttpUtility.UrlEncode(email);
        var scheme = "https"; // todo: взять из Options
        var host = "auth.runex.space";

        return $"{scheme}://{host}/api/reset-password?email={encodedEmail}&token={encodedToken}";
    }
}
