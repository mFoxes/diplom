using Singularis.Internal.Domain.Entities;

namespace Domain.Models;

public class BookingHistory : IEntity<Guid>
{
    public Guid Id { get; set; }
    public Guid DeviceId { get; set; }
    public string TakedBy { get; set; }
    public DateTime TakeAt { get; set; }
    public DateTime? ReturnedAt { get; set; }
    public bool IsDeleted { get; set; }
}