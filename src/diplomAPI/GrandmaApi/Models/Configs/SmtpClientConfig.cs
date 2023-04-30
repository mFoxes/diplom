namespace GrandmaApi.Models.Configs;

public class SmtpClientConfig
{
    public string SmtpAddress { get; set; }
    public int Port { get; set; }
    public bool UseSsl { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Address { get; set; }
}