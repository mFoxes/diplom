using Domain.Enums;

namespace LdapConnector;

public class User
{
    public int LdapId { get; set; }
    public string CommonName { get; set; }
    public string UId { get; set; }
    public string Email { get; set; }
    public Roles Role { get; set; }
    
}