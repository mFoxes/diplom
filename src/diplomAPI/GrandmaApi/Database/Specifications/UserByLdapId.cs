using Domain.Models;
using Singularis.Internal.Domain.Specification;

namespace GrandmaApi.Database.Specifications;

public class UserByLdapId : Specification<MongoUser>
{
    public List<int> LdapIds { get; } = new();
    public UserByLdapId(int ldapId)
    {
        LdapIds.Add(ldapId);
        Query = Source().Where(u => LdapIds.Contains(u.LdapId));
    }

    public UserByLdapId(ICollection<int> ldapIds)
    {
        LdapIds.AddRange(ldapIds);
        Query = Source().Where(u => LdapIds.Contains(u.LdapId));
    }
}