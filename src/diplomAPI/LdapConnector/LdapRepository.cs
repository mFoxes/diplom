using Domain.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Novell.Directory.Ldap;

namespace LdapConnector;

public class LdapRepository : ILdapRepository, IDisposable
{
    private readonly IOptions<LdapConfig> _config;
    private readonly ILogger<LdapRepository> _logger;
    private readonly LdapConnection _connection;

    public LdapRepository(IOptions<LdapConfig> config, ILogger<LdapRepository> logger)
    {
        _config = config;
        _logger = logger;
        _connection = new LdapConnection();
        Connect();
    }

    public User GetUser(string login, string password)
    {
        var configValue = _config.Value;
        var filter = string.Format(configValue.VerifyEmployeeFilter, login, password);
        var attrs = new[] { "mail", "cn", "uidNumber", "uid", "gidNumber" };
        
        var res = _connection.Search(configValue.BasePath, LdapConnection.ScopeSub, filter, attrs, false);
        
        if (res.HasMore())
        {
            var entry = res.Next();
            return MapLdapEntry(entry);
        }
        return null;
    }

    public IEnumerable<User> GetUsers()
    {
        var configValue = _config.Value;
        var users = new List<User>();
        var attrs = new[] { "mail", "cn", "uidNumber", "uid", "gidNumber" };
        var res = _connection.Search(configValue.BasePath, LdapConnection.ScopeSub, configValue.EmployeesFilter, attrs, false);
        
        while (res.HasMore())
        {
            var user = MapLdapEntry(res.Next());
            if (user != null)
            {
                users.Add(user);   
            }
        }
        return users;
    }

    private User MapLdapEntry(LdapEntry entry)
    {
        var ldapId = int.Parse(entry.GetAttribute("uidNumber").StringValue);
        var uid = entry.GetAttribute("uid").StringValue;
        var name = entry.GetAttribute("cn").StringValue;
        var gid = int.Parse(entry.GetAttribute("gidNumber").StringValue);
        if (!entry.GetAttributeSet().ContainsKey("mail"))
        {
            _logger.LogWarning($"У пользователя с Id {ldapId} : {name} не указана почта. Пользователя пропущен");
            return null;
        }

        return new User()
        {
            CommonName = name,
            Email = entry.GetAttribute("mail").StringValue,
            LdapId = ldapId,
            UId = uid,
            Role = gid == _config.Value.AdminGid ? Roles.Admin : Roles.Employee
        };
    }
    public void Dispose()
    {
        _connection.Disconnect();
    }
    private void Connect()
    {
        var configValue = _config.Value;
        _connection.Connect(configValue.Host, configValue.Port);
        _connection.Bind(configValue.BindDN, configValue.BindPassword);
    }
    
}