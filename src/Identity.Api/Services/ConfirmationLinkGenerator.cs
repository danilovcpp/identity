using System.Web;
using Identity.Application.Abstractions;

namespace Identity.Api.Services;

public class ConfirmationLinkGenerator : IConfirmationLinkGenerator
{
    public string CreateConfirmationLink(string userId, string token)
    {
        var encodedToken = HttpUtility.UrlEncode(token);
        var scheme = "https"; // todo: взять из Options
        var host = "auth.runex.space";

        return $"{scheme}://{host}/api/confirm-email?userId={userId}&token={encodedToken}";
    }
}