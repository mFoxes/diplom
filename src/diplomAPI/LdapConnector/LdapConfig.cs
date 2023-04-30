namespace LdapConnector;

public class LdapConfig
{
    public string Host { get; set; }
    public int Port { get; set; }
    public string BindDN { get; set; }
    public string BindPassword { get; set; }
    public string BasePath { get; set; }
    public string VerifyEmployeeFilter { get; set; }
    public string EmployeesFilter { get; set; }
    public int AdminGid { get; set; }
}