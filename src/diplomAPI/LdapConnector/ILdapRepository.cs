namespace LdapConnector;

public interface ILdapRepository
{
    User GetUser(string login, string password);
    IEnumerable<User> GetUsers();
}