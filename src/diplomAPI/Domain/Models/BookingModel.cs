using Domain.Enums;
using Singularis.Internal.Domain.Entities;


namespace Domain.Models;

public class BookingModel : IEntity<Guid>
{
    public Guid Id { get; set; }
    public Guid DeviceId {get;set;}
    public DeviceStates State { get; set; }
    public Guid? UserId { get; set; }
    public DateTime? TakeAt { get; set; }
    public DateTime? ReturnAt { get; set; }
    public bool IsDeleted { get; set; }
}