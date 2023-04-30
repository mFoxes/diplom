namespace GrandmaApi.Models.MessageModels;

public class DeviceTookMessage
{
    public Guid UserId { get; set; }
    public Guid DeviceId { get; set; }
    public DateTime ReturnAt { get; set; }
}