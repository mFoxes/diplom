using System.Security.Claims;
using Duende.IdentityServer.Models;
using LdapConnector;

namespace AuthServer.Auth;

public class IdentityServerConfig
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new[]
        {
            new IdentityResources.OpenId()
        };
    public static IEnumerable<ApiResource> ApiResources =>
        new[]
        {
            new ApiResource("WebApi", new[] {ClaimTypes.Role, ClaimTypes.UserData} )
            {
                Scopes = new[] {"WebApi"}
            }
        };

    public static IEnumerable<Client> Clients =>
        new[]
        {
            new Client {
                ClientId = "grandmaSpaClient",
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                AllowOfflineAccess = true,
                RequireClientSecret = false,
                AllowedScopes = { "WebApi"}
            }
        };
    public static IEnumerable<ApiScope> ApiScopes =>
        new[]
        {
            new ApiScope("WebApi")
        };

}