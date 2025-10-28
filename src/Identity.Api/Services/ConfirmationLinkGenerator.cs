using System.Web;
using Identity.Api.Abstractions;

namespace Identity.Api.Services;

public class ConfirmationLinkGenerator : IConfirmationLinkGenerator
{
    public string CreateConfirmationLink(string userId, string token)
    {
        var encodedToken = HttpUtility.UrlEncode(token);
        var scheme = ""; // todo: взять из Options
        var host = "";

        return $"{scheme}://{host}/api/confirm-email?userId={userId}&token={encodedToken}";
    }
}