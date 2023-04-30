using Domain.Models;
using Singularis.Internal.Domain.Specification;

namespace GrandmaApi.Database.Specifications
{
    public class FilterDeviceByName : Specification<DeviceModel>
    {
        public FilterDeviceByName(string name)
        {
            Query = Source().Where(d => d.Name.ToLower().Contains(name.ToLower()));
        }
    }
}