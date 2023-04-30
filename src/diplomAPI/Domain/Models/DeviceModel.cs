using Singularis.Internal.Domain.Entities;

namespace Domain.Models;

public class DeviceModel : IEntity<Guid>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string InventoryNumber { get; set; }
    public Guid? PhotoId { get; set; }
    public bool IsDeleted { get; set; }
}
