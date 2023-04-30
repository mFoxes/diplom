using Domain.Enums;

namespace DTO;

public class BookingDto
{
    public Guid Id { get; set; }
    public Guid DeviceId { get; set; }
    public string Name { get; set; }
    public string InventoryNumber { get; set; }
    public DeviceStates State { get; set; }
    public Guid? UserId { get; set; }
    public string TakedBy { get; set; }
    public DateTime? TakeAt { get; set; }
    public DateTime? ReturnAt { get; set; }
}