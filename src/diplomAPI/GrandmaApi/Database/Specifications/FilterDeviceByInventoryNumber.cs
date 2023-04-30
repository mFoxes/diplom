using Domain.Models;
using Singularis.Internal.Domain.Specification;

namespace GrandmaApi.Database.Specifications
{
    public class FilterDeviceByInventoryNumber : Specification<DeviceModel>
    {
        public FilterDeviceByInventoryNumber(string inventoryNumber)
        {
            Query = Source().Where(d => d.InventoryNumber.ToLower().Contains(inventoryNumber.ToLower()));
        }
    }
}