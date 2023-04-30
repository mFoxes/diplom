using Domain.Enums;

public class DeviceInfoDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DeviceStates State { get; set; }
}