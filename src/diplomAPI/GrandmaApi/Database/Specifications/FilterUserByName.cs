using Domain.Models;
using Singularis.Internal.Domain.Specification;

namespace GrandmaApi.Database.Specifications
{
    public class FilterUserByName : Specification<MongoUser>
    {
        public FilterUserByName(string name)
        {
            Query = Source().Where(u => u.CommonName.ToLower().Contains(name.ToLower()));
        }
    }
}