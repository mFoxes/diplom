using Domain.Enums;

namespace DTO;

public class UpdateBookingDto
{
    public Guid Id { get; set; }
    public DeviceStates State { get; set; }
    public Guid? UserId { get; set; }
    public DateTime TakeAt { get; set; }
    public DateTime ReturnAt { get; set; }
}