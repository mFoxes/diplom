using Domain.Enums;
using Singularis.Internal.Domain.Entities;

namespace Domain.Models;

public class MongoUser : IEntity<Guid>
{
    public Guid Id { get; set; }
    public int LdapId { get; set; }
    public string CommonName { get; set; }
    public string Email { get; set; }
    public Guid? PhotoId { get; set; }
    public string MattermostName { get; set; }
    public double[] Embeddings { get; set; }
    public bool IsDeleted { get; set; }
}