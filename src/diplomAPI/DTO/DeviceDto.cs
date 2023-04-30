namespace DTO;

public class DeviceDto
{
    public Guid Id { get; set; } = Guid.Empty;
    public string Name { get; set; } = string.Empty;
    public string InventoryNumber { get; set; } = string.Empty;
    public Guid? PhotoId { get; set; }
}