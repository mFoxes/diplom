using Singularis.Internal.Domain.Entities;

namespace Domain.Models;

public class FileModel : IEntity<Guid>
{
    public Guid Id { get; set; }
    public string StorredFileName { get; set; }
    public bool IsDeleted { get; set; }
}