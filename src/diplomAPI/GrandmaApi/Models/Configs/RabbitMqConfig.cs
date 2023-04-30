namespace GrandmaApi.Models.Configs;

public class RabbitMqConfig
{
    public string ConnectionUri { get; set; }
    public string GrandmaQueue { get; set; }
    public string GrandmaNamesQueue { get; set; }
    public string GrandmaPersonsQueue { get; set; }
}