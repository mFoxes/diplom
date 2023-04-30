using System.Security.Claims;

namespace GrandmaApi.Extensions;

public static class HttpContextExtensions
{
    public static string GetLdapId(this HttpContext context)
    {
        var contextUser = (ClaimsIdentity)context.User.Identity;
        return contextUser?.FindFirst(ClaimTypes.UserData)?.Value;
    }
    public static string GetUserRole(this HttpContext context)
    {
        var contextUser = (ClaimsIdentity)context.User.Identity;
        return contextUser?.FindFirst(ClaimTypes.Role)?.Value;
    }
}